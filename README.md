# MergeMechanic

Vuvy Unity Developer case study: grid tabanlı bir merge oyununun temel mekanikleri.

- Unity 6000.0.62f1, URP
- UniTask, DOTween, Lean Touch

## "Üret" butonu yerine spawner

- Case, ekranda bir "Üret" butonu istiyor; butona basınca rastgele bir zincirden Seviye 1 item boş bir hücreye yerleşmeli. Buton yerine grid üzerinde duran spawner'lar kullanıyorum; merge oyunlarında üretim genelde böyle yapılıyor.
- Spawner tahtada bir hücre kaplıyor. Dokunulunca kendi zincirlerinden birinin Seviye 1 item'ını rastgele boş bir hücreye koyuyor; grid doluysa üretmiyor ve uyarı veriyor. Böylece butondan beklenen davranış korunuyor.
- Tek bir buton bütün zincirlerden üretirdi. Spawner'larla hangi zincirin hangi oranda üretileceği her spawner için ayrı ayarlanabiliyor; bir spawner birden fazla zincir tutabiliyor.
- Hangi spawner'ların oyunda olacağı ve başlangıçta hangi hücrede duracakları koddan değil, `GameplayConfig`'ten geliyor.

## Klasör ve assembly yapısı

```
Scripts/
  Core/           Game.Core            oyundan bağımsız altyapı (ServiceLocator, EventBus, BaseService)
  Events/         Game.Events          bütün event'ler
  <X>System/      Game.<X>System       oyun sistemleri: ItemSystem, SpawnerSystem, SceneSystem …
  UI/<Ekran>/     Game.UI.<Ekran>      arayüz ekranları: LoadingScreen
  UI/Common/      Game.UI.Common       birden fazla ekranda kullanılan arayüz parçaları
                  her birinin içi: Interfaces/ · Data/ · Controllers/ · Views/
```

- Kodu katmanlara göre (Data, Gameplay, UI gibi) de bölebilirdim. Ama o zaman tek bir sistem birkaç klasöre dağılır, bir sistemin neyden oluştuğu tek yerde görünmezdi. Bu yüzden klasörler sisteme göre; bir sisteme ait her şey kendi klasöründe.
- Her sistem kendi assembly'sinde. Bağımlılıklar tek yönlü ve derleyici tarafından korunuyor; örneğin `GameplaySystem` → `SpawnerSystem` → `ItemSystem`, tersi derlenmiyor.
- Interface'ler, event'ler ve veri tipleri `public`, implementasyonlar `internal`. Başka bir sistem sözleşmeyi görebiliyor ama somut sınıfı göremiyor.
- `Data` sınıfları sadece veri tutuyor, kural içermiyor. Kurallar o veriyi kullanan controller'da.
- Bütün event'ler ayrı bir `Game.Events` assembly'sinde. Event'ler kendi sistemlerinde dursaydı, dinleyen sistem event'i atan sisteme bağımlı olmak zorunda kalırdı.
- Assembly referansları GUID ile veriliyor; bir assembly yeniden adlandırılınca referanslar kopmuyor.
- Kullanılmayan klasör, sınıf ve assembly referansı tutulmuyor; ihtiyaç doğduğunda ekleniyor.
- Sahnelerde duran her obje bir prefab. Prefab klasörleri script klasörleriyle aynı yapıda (`Prefabs/MainMenuSystem` ↔ `Scripts/MainMenuSystem`); script karşılığı olmayan kameralar `Prefabs/CameraSystem`'de.

## Bağımlılık yönetimi

- DI için Zenject kullanabilirdim. Ama hem öğrenme eşiği hem de açılış maliyeti bu boyuttaki bir proje için fazla olduğu için basit bir `ServiceLocator` kullanıyorum.
- `ServiceLocator`, Splash sahnesindeki `ProjectServiceLocator` prefab'ının kökünde `MonoSingleton` olarak duruyor. `DontDestroyOnLoad` olduğu için proje boyunca yaşayan servisler (örneğin `SceneLoader`) sahne geçişlerinde kaybolmuyor.
- `*ServiceLocator` prefab'larında sadece servisler duruyor; her servis kendi adıyla bir child objede, böylece hangi servislerin olduğu Hierarchy'de görünüyor. Servis olmayan objeler (`FrameRateController`, loading ekranı) sahnede kendi prefab'larıyla duruyor.
- Servislere interface üzerinden erişiliyor (`ISceneLoader`), somut sınıflara doğrudan bağımlılık yok.
- Servislerin kayıt sırası önemli olmasın diye: `Awake`'te sadece kayıt (`Bind`), başka servislere erişim (`Get`) `Start`'tan itibaren. Unity bütün `Awake`'leri `Start`'lardan önce çağırdığı için execution order ayarlamak gerekmiyor.

## Event sistemi

- Event'ler birbirinden bağımsız sistemlerin haberleşmesi için. Atan taraf kimin dinlediğini bilmiyor; örneğin loading ekranı sahne yüklemesini kimin başlattığını bilmeden açılıp kapanıyor.
- Komutlar event ile değil, doğrudan çağrıyla. "Oyna" butonu bir event atabilirdi, ama dinleyen olmasaydı sessizce hiçbir şey olmazdı; bu yüzden `ISceneLoader.LoadGameplayAsync()`'i doğrudan çağırıyor.
- Event'ler class yerine `readonly struct`; sık atılan event'lerde allocation ve GC yükü oluşmuyor. Ortak taban sınıf yerine `IEvent` marker interface'i yeterli.
- `Fire` sırasında bir handler abonelikten çıkabilsin diye abone listesi sondan başa dolaşılıyor. Bir handler hata verirse hata loglanıyor, diğer handler'lar çalışmaya devam ediyor.

## Oyun akışı

- Sahneler: Splash → MainMenu → Gameplay. Sahneler additive değil, tek tek yükleniyor; iki sahne aynı anda açık olsaydı kameralar, EventSystem'ler ve sahneye ait servisler çakışırdı.
- Splash kısa bir süre görünüp ana menüyü açıyor. Splash'te kamera yok; ekran `Screen Space - Overlay` canvas ile çiziliyor.
- Ana menüde sadece "Oyna" butonu var. Gameplay'deki "Menü" butonu ana menüye dönüyor.
- Loading ekranı Splash'te kendi prefab'ıyla duruyor, sahne değişirken kaybolmaması için kendini `DontDestroyOnLoad` yapıyor. Her yüklemenin başında (`SceneLoadStartedEvent`) açılıp yükleme bitince (`SceneLoadCompletedEvent`) kapanıyor.
- "Ana menü yüklendi" ve "level hazır" diye iki ayrı event tutabilirdim. Ama şu an iki sahne de yüklendiği anda kullanıma hazır; ikisini de `SceneLoader` atıyor ve tek bir event yetiyor. Gameplay'in hazırlanması bekleme gerektirirse, hazır olduğunu Gameplay'in kendisi duyuracak.
- Canvas'lar 1080×2400 referans çözünürlüğünde ve `Expand` modunda; UI hiçbir ekran oranında taşmıyor. Arayüz cihazın safe area'sına yerleşiyor.

## Item tanım yapısı

- Item'ları zincirler halinde tanımlıyorum. Her zincir bir ScriptableObject (`ItemChainData`) ve sıralı bir level listesi tutuyor; her level'ın sprite'ı ve merge skoru var.
- Item'lar kod içinde bir enum ve tabloyla da tanımlanabilirdi. Ama o zaman her yeni item kod değişikliği demekti. ScriptableObject ile zincirler Inspector'dan ekleniyor, sprite'lar sürüklenip bırakılıyor.
- Her item'ı ayrı bir asset olarak da tanımlayabilirdim; her item bir sonraki level'ı gösterirdi. Ama 5 level'lık bir zincir 5 ayrı asset olur, aradaki bağlantılar elle kurulurdu ve bir halka unutulunca zincir kopardı. Zincirin bütün level'ları tek asset'te ve sıralı; sıradaki level, listedeki sıradaki eleman.
- Bir item'ın kimliği sadece zinciri ve level'ı. Max level ayrıca girilmiyor, zincirdeki level sayısından geliyor; iki ayrı yerde tutulsaydı birbiriyle çelişebilirdi.
- Merge skoru ayrı bir skor tablosunda da tutulabilirdi. Ama skor item'ın bir özelliği; level'ın içinde durunca yeni bir zincir kendi skorlarıyla birlikte ekleniyor ve zincirler farklı puan verebiliyor. Skor, merge sonucu oluşan level'ın skoru. Level 1 merge ile oluşmadığı için skoru 0.
- Spawner'lar da ScriptableObject (`SpawnerData`): generator görseli ve hangi zincirden hangi ağırlıkla üretileceği. Ağırlıklı seçim kuralı data'da değil, spawner'ı kullanan tarafta.
- Grid boyutu ve başlangıçta hangi spawner'ın hangi hücrede duracağı `GameplayConfig`'te. Konumlar dünya koordinatı değil grid hücresi (`Vector2Int`); tasarlayan kişi "3. sütun, 5. satır" diye düşünüyor.
- Şu an 10 zincir, her birinde 5 level ve her zincir için bir spawner var. Sprite'lar 256 piksel ve 256 PPU; bir item tam bir hücre (1 birim) kaplıyor.

**Yeni bir zincir eklemek (kod değişikliği yok):**

1. `Create > Game > Item Chain` ile asset oluştur; level'ları sırayla ekle, her birine sprite ve merge skoru ver.
2. Zinciri mevcut bir spawner'ın `Chains` listesine ekle ya da `Create > Game > Spawner` ile yeni bir spawner oluştur.
3. Yeni spawner'ı `GameplayConfig`'teki `Spawners` listesine bir başlangıç hücresiyle ekle.

## Tasarım kalıpları

| Kalıp | Nerede | Neden | Alternatif |
|---|---|---|---|
| Service Locator | `ServiceLocator`, `ISceneLoader` | Servislere interface üzerinden, kurulum maliyeti olmadan erişim | Zenject gibi bir DI container; bu boyutta öğrenme ve açılış maliyeti fazla |
| Singleton | `MonoSingleton` (`ServiceLocator`) | Proje boyunca tek bir locator, sahne geçişlerinde yaşıyor | Saf statik sınıf; ama o zaman Splash'teki servislerle birlikte bir prefab'da duramazdı |
| Observer (Event Bus) | `EventBus`, `Game.Events` | Atan ve dinleyen birbirini tanımıyor (loading ekranı, sahne yükleme) | C# event'leri; dinleyenin atan sınıfa referans tutması gerekirdi |
| Type Object | `ItemChainData`, `SpawnerData` | Item ve spawner türleri sınıf değil veri; yeni tür kodsuz ekleniyor | Her item türü için alt sınıf ya da enum + switch; her yeni tür kod değişikliği |

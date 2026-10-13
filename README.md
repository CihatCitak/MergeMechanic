# MergeMechanic

Grid tabanlı bir merge oyununun temel mekanikleri.

## Nasıl çalıştırılır

- Unity 6000.0.62f1, URP (2D, Universal renderer).
- Kullanılan kütüphaneler: UniTask (async), DOTween (animasyon), Lean Touch (dokunma).
- `Assets/Scenes/Splash.unity` sahnesini açıp Play'e bas. Oyun her zaman Splash'ten başlamalı; proje boyunca yaşayan servisler (sahne yükleme, kayıt) orada kuruluyor. Akış: Splash → ana menü → "Oyna" → oyun.
- Oyun her hamlede kaydediliyor ve kaldığı yerden devam ediyor. Kaydı silmek için oyunda sağ üstteki sıfırlama butonu ya da editörde `MergeMechanic > Reset Player Data` menüsü (play mode dışında).

### Testler

- EditMode testleri `Assets/Tests/Editor`'da; `Window > General > Test Runner > EditMode > Run All` ile koşuluyor (30 test).
- Kapsadıkları: `EventBus`, `ServiceLocator`, frame rate seçimi, grid hesapları (`GridMath`), hücre durumları (`GridObjectHolder`), merge kuralı (`MergeRules`), zincir kataloğu (`ItemCatalog`) ve kayıt dosyasının yazılıp okunması (bozuk ve eksik dosya dahil).
- Kayıt testleri kendi test dosyalarını kullanıyor; oyunun `player.json`'ına dokunmuyor.

### Build

- Hedef Android: IL2CPP, ARM64, dikey ekran, paket adı `com.DefaultCompany.MergeMechanic`, sürüm 1.0.0.
- `File > Build Profiles > Android > Build`. Build'deki sahneler sırasıyla Splash, MainMenu, Gameplay. APK'ler repoda tutulmuyor (`.gitignore`).

## Mimari

Sistem (feature) bazlı modüler bir yapı. Her oyun sistemi kendi assembly'sinde; sistemler birbirine interface ve event üzerinden bağlı, içerik ve ayarlar ScriptableObject'te, kurallar Unity'den bağımsız saf fonksiyonlarda. Başlıca assembly'ler ve bağımlılıkları (üçüncü parti kütüphaneler hariç):

- **Modüller:** Her sistem ayrı bir `.asmdef` ve içi `Interfaces/`, `Data/`, `Controllers/`, `Views/` olarak bölünmüş. Bağımlılık grafiği tek yönlü ve döngüsüz; bunu derleyici zorluyor, ters yönde bir referans derlenmiyor. Sözleşmeler (interface, event, veri tipleri) `public`, implementasyonlar `internal`; bir sistem başka bir sistemin somut sınıfını derleme zamanında göremiyor.
- **Bağımlılık çözümü:** Servisler `BaseService`'ten türeyen MonoBehaviour'lar. `Awake`'te kendilerini interface tipleriyle statik `ServiceLocator`'a bağlıyor (`Bind<IGrid>(this)`), `OnDestroy`'da kaldırıyor; diğer servislere `Start`'tan itibaren `ServiceLocator.Get<T>()` ile erişiliyor. Böylece kayıt sırası önemli değil ve execution order ayarı gerekmiyor. Servislerin iki ömrü var: proje boyunca yaşayanlar `ProjectServiceLocator`'da (`DontDestroyOnLoad`; `SceneLoader`, `PlayerDataStore`), oyuna ait olanlar `GameplayServiceLocator`'da (grid, hücre durumu, item, vurgu, toast, popup, skor) ve sahneyle birlikte doğup ölüyor. Aynı sistemin parçaları birbirine prefab'da serialize referansla bağlı.
- **Haberleşme:** İki kanal var. Komutlar doğrudan interface çağrısı (`IItemCreator.Create`, `ISceneLoader.LoadGameplayAsync`); çağıran sonucun olacağını biliyor. Bildirimler tip güvenli bir `EventBus` üzerinden (`readonly struct : IEvent`, örneğin `ItemMergedEvent`, `ItemPlacedEvent`, `SceneLoadStartedEvent`); atan dinleyeni tanımıyor ve struct olduğu için atarken allocation yok. Event'ler ayrı bir `Game.Events` assembly'sinde; dinleyen sistem atan sisteme bağımlı olmuyor.
- **Veri:** Oyun içeriği Type Object olarak ScriptableObject'te (`ItemChainData`, `SpawnerData`); sayısal ayarlar config asset'lerinde (bkz. Config). Kod içeriği değil kuralı biliyor; yeni bir zincir ya da denge değişikliği kod değişikliği gerektirmiyor. Kalıcı veri ayrı: tek bir JSON dosyası (`PlayerData`) ve onun tek sahibi `PlayerDataStore` (`IPlayerProgress`).
- **Durum, görünüm ve kural ayrımı:** Durumun tek bir sahibi var: hangi hücrede ne olduğu `GridObjectHolder`'da, skor ve kayıtlı tahta `IPlayerProgress`'te. Görünüm ayrı bileşenlerde (`ItemView`, `ScoreView`, `CellHighlighter`) ve sadece durumu gösteriyor. Kurallar (`MergeRules`, `GridMath`) Unity'ye bağlı olmayan saf statik fonksiyonlar ve EditMode testleriyle kontrol ediliyor.
- **Async ve yaşam döngüsü:** Animasyonlar DOTween, beklemeler UniTask. Beklenen tween'ler objenin `destroyCancellationToken`'ına `KillAndCancelAwait` ile bağlı; obje yok olunca (örneğin sahne değişince) bekleyen akış tamamlanmış sayılmıyor, iptal ediliyor ve yarım kalan bir yerleşme yok olmuş bir objeye uygulanmıyor.

Her başlığın ayrıntıları, neden seçildiği ve alternatifleri aşağıdaki bölümlerde.

## Config

Oyunun ayarları koda yazılmıyor; ScriptableObject asset'lerinden geliyor ve Inspector'dan değiştirilebiliyor. Config'ler `Assets/ScriptableObjects/Configs`'te, zincirler `Assets/ScriptableObjects/ItemChains`'te, spawner'lar `Assets/ScriptableObjects/Spawners`'ta.

### Grid (`GridConfig`)

| Alan | Değer | Ne değiştirir |
|---|---|---|
| Size | 5×5 | Grid'in sütun ve satır sayısı. Kamera ve spawner sırası yeni boyuta kendiliğinden uyuyor. |
| Cell Size | 1 | Bir hücrenin dünya birimi cinsinden boyutu. |
| Corner Padding | 0.25 | Tahta kenarlığının hücrelerden ne kadar dışarıda durduğu. |

### Kamera (`CameraConfig`)

| Alan | Değer | Ne değiştirir |
|---|---|---|
| Padding | 0.3 | Tahta ile ekran kenarı arasında bırakılan boşluk. |

### Item zincirleri (`ItemChainData`, `ItemCatalog`)

| Alan | Ne değiştirir |
|---|---|
| Id | Zincirin kayıttaki kimliği; benzersiz olmalı ve oyun yayındayken değiştirilmemeli (eski kayıtlardaki item'lar bulunamaz). |
| Levels | Zincirin level'ları, sırasıyla. Max level bu listenin uzunluğu. |
| Levels → Sprite | O level'ın görseli. |
| Levels → Merge Score | Merge sonucunda bu level oluşunca kazanılan puan (şu an L2 = 10, L3 = 25, L4 = 50, L5 = 100). |
| `ItemCatalog` → Chains | Kayıttan yüklenebilecek bütün zincirler; yeni bir zincir buraya da eklenmeli. |

### Spawner'lar (`SpawnerData`, `SpawnerBarConfig`, `SpawnerConfig`)

| Alan | Değer | Ne değiştirir |
|---|---|---|
| `SpawnerData` → Sprite, Chain | | Spawner'ın görseli ve hangi zincirden ürettiği. |
| `SpawnerBarConfig` → Spawners | Bakery, Coffee, Flowers | Oyundaki spawner'lar ve her birinin ürettiği level (Spawn Level, şu an hepsi 1). |
| `SpawnerBarConfig` → Spawner Size | 1.8 | Spawner'ların boyutu. |
| `SpawnerBarConfig` → Spacing | 2 | Spawner'lar arasındaki mesafe. |
| `SpawnerBarConfig` → Grid Gap | 0.4 | Spawner sırası ile tahta arasındaki boşluk. |
| `SpawnerConfig` → Grid Full Message | "Tahta dolu!" | Boş hücre yokken çıkan uyarı. |
| `SpawnerConfig` → Tap Punch, Tap Punch Duration | 0.15, 0.2 | Dokunulunca spawner'ın büyüme miktarı ve süresi. |

### Item animasyonları (`ItemAnimationConfig`)

| Alan | Değer | Ne değiştirir |
|---|---|---|
| Spawn Duration, Spawn Jump Power, Spawn Ease, Spawn Start Scale | 0.45, 1, OutQuad, 0.7 | Spawner'dan hücreye zıplayarak gitme animasyonu. |
| Move Duration, Move Ease | 0.15, OutQuad | Boş hücreye taşınma ve hücreye geri dönme. |
| Merge Duration, Merge Ease, Merge Start Scale | 0.3, OutBack, 0.5 | Merge sonucu oluşan item'ın büyüyerek belirmesi. |
| Reject Duration, Reject Strength, Reject Vibrato | 0.3, 0.08, 14 | Birleşemeyen bir item'ın üstüne bırakılınca sallanma. |

### Sürükleme ve vurgu (`ItemDragConfig`, `CellHighlighterConfig`)

| Alan | Değer | Ne değiştirir |
|---|---|---|
| `ItemDragConfig` → Drag Sorting Order | 20 | Sürüklenen item'ın çizim sırası; diğer her şeyin üstünde kalmalı. |
| `ItemDragConfig` → Max Level Message | "Maksimum seviye!" | Max level item'lar üst üste bırakılınca çıkan uyarı. |
| `CellHighlighterConfig` → Valid Color, Invalid Color | yeşil, kırmızı | Sürüklerken hedef hücrenin birleşebilir ve birleşemez rengi. |

### Skor (`ScoreConfig`)

| Alan | Değer | Ne değiştirir |
|---|---|---|
| Flight Duration, Flight Ease | 0.6, InQuad | Merge noktasından skor barına uçan yıldız. |
| Icon Punch, Icon Punch Duration | 0.25, 0.2 | Yıldız varınca skor ikonunun büyüme animasyonu. |

### Arayüz (`ToastConfig`, `PopupConfig`, `UIConfig`)

| Alan | Değer | Ne değiştirir |
|---|---|---|
| `ToastConfig` → Fade Duration, Visible Duration | 0.2, 1.2 | Uyarıların açılıp kapanma ve ekranda kalma süresi. |
| `PopupConfig` → Open Duration, Close Duration, Start Scale, Open Ease | 0.25, 0.15, 0.8, OutBack | Popup'ın açılış ve kapanış animasyonu. |
| `PopupConfig` → Dim Alpha | 0.5 | Popup arkasındaki karartmanın koyuluğu. |
| `UIConfig` → Press Scale, Press Duration | 0.92, 0.08 | Butona basılınca küçülme miktarı ve süresi. |

### Performans (`FrameRateConfig`)

| Alan | Değer | Ne değiştirir |
|---|---|---|
| High Frame Rate, Low Frame Rate | 60, 30 | Güçlü ve zayıf cihazlarda hedef FPS. |
| Min Memory Mb, Min Processor Count | 3500, 6 | Bir cihazın güçlü sayılması için gereken bellek ve çekirdek sayısı. |

### Prefab'daki ayarlar

| Nerede | Değer | Ne değiştirir |
|---|---|---|
| `SplashController` → Duration | 1 | Splash ekranının görünme süresi (saniye). |
| `ProjectServiceLocator` → `SceneLoader` → Main Scene Name, Gameplay Scene Name | MainMenu, Gameplay | Yüklenecek sahnelerin adları. |

## "Üret" butonu yerine spawner

- Case, ekranda bir "Üret" butonu istiyor; butona basınca rastgele bir zincirden Seviye 1 item boş bir hücreye yerleşmeli. Buton yerine tahtanın altında bir sıra spawner kullanıyorum; merge oyunlarında üretim genelde böyle yapılıyor.
- Her spawner tek bir zincire ait. Dokununca o zincirin item'ı rastgele boş bir hücreye yerleşiyor; grid doluysa üretmiyor ve uyarı veriyor. Böylece butondan beklenen davranış korunuyor; hangi zincirden üretileceğini rastgele seçim yerine oyuncu, dokunduğu spawner'la seçiyor.
- Hangi spawner'ların oyunda olacağı ve her birinin kaçıncı seviye item ürettiği koddan değil, `SpawnerBarConfig`'ten geliyor.

## Klasör ve assembly yapısı

```
Scripts/
  Core/           Game.Core            oyundan bağımsız altyapı (ServiceLocator, EventBus, BaseService)
  Events/         Game.Events          bütün event'ler
  <X>System/      Game.<X>System       oyun sistemleri: ItemSystem, SpawnerSystem, SceneSystem …
  UI/<Ekran>/     Game.UI.<Ekran>      arayüz ekranları: LoadingScreen, Toast
  UI/Common/      Game.UI.Common       birden fazla ekranda kullanılan arayüz parçaları
                  her birinin içi: Interfaces/ · Data/ · Controllers/ · Views/
```

- Kodu katmanlara göre (Data, Gameplay, UI gibi) de bölebilirdim. Ama o zaman tek bir sistem birkaç klasöre dağılır, bir sistemin neyden oluştuğu tek yerde görünmezdi. Bu yüzden klasörler sisteme göre; bir sisteme ait her şey kendi klasöründe.
- Her sistem kendi assembly'sinde. Bağımlılıklar tek yönlü ve derleyici tarafından korunuyor; örneğin `SpawnerSystem` → `ItemSystem` → `GridSystem`, tersi derlenmiyor.
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
- Ana menüde sadece "Oyna" butonu var. Gameplay'de sol üstteki geri butonu ana menüye dönüyor, sağ üstteki buton oyunu sıfırlıyor (bkz. Kayıt).
- Loading ekranı Splash'te kendi prefab'ıyla duruyor, sahne değişirken kaybolmaması için kendini `DontDestroyOnLoad` yapıyor. Her yüklemenin başında (`SceneLoadStartedEvent`) açılıp yükleme bitince (`SceneLoadCompletedEvent`) kapanıyor.
- "Ana menü yüklendi" ve "level hazır" diye iki ayrı event tutabilirdim. Ama şu an iki sahne de yüklendiği anda kullanıma hazır; ikisini de `SceneLoader` atıyor ve tek bir event yetiyor. Gameplay'in hazırlanması bekleme gerektirirse, hazır olduğunu Gameplay'in kendisi duyuracak.
- Canvas'lar 1080×2400 referans çözünürlüğünde ve `Expand` modunda; UI hiçbir ekran oranında taşmıyor. Arayüz cihazın safe area'sına yerleşiyor.
- Yazılar Türkçe karakterleri de içeren statik bir font atlasıyla çiziliyor (`Art/Fonts/LiberationSans SDF - TR`; ASCII ve ÇĞİıÖŞÜ). TMP'nin dinamik fallback'i de bu karakterleri üretebiliyordu; ama dinamik atlas oyun çalışırken büyüyüp asset'i değiştiriyor, cihazda da her karakterin ilk kullanımında üretme maliyeti oluyor. Yeni bir karakter gerekirse atlasa eklenmesi gerekiyor.
- Butonlar diğer görsellerle aynı kurallarla çizilmiş 9-slice bir sprite (`button_9slice`) kullanıyor. Sprite gri tonlu, rengi `Image.color`'dan geliyor; her renk için ayrı sprite çizmek yerine aynı sprite farklı renkteki butonlarda kullanılıyor.

## Item tanım yapısı

- Item'ları zincirler halinde tanımlıyorum. Her zincir bir ScriptableObject (`ItemChainData`) ve sıralı bir level listesi tutuyor; her level'ın sprite'ı ve merge skoru var.
- Item'lar kod içinde bir enum ve tabloyla da tanımlanabilirdi. Ama o zaman her yeni item kod değişikliği demekti. ScriptableObject ile zincirler Inspector'dan ekleniyor, sprite'lar sürüklenip bırakılıyor.
- Her item'ı ayrı bir asset olarak da tanımlayabilirdim; her item bir sonraki level'ı gösterirdi. Ama 5 level'lık bir zincir 5 ayrı asset olur, aradaki bağlantılar elle kurulurdu ve bir halka unutulunca zincir kopardı. Zincirin bütün level'ları tek asset'te ve sıralı; sıradaki level, listedeki sıradaki eleman.
- Bir item'ın kimliği sadece zinciri ve level'ı. Max level ayrıca girilmiyor, zincirdeki level sayısından geliyor; iki ayrı yerde tutulsaydı birbiriyle çelişebilirdi.
- Her zincirin kayıtta kullanılan bir `Id`'si var ve bütün zincirler `ItemCatalog`'ta listeleniyor (bkz. Kayıt).
- Merge skoru ayrı bir skor tablosunda da tutulabilirdi. Ama skor item'ın bir özelliği; level'ın içinde durunca yeni bir zincir kendi skorlarıyla birlikte ekleniyor ve zincirler farklı puan verebiliyor. Skor, merge sonucu oluşan level'ın skoru. Level 1 merge ile oluşmadığı için skoru 0.
- Spawner'lar da ScriptableObject (`SpawnerData`): generator görseli ve hangi zincirden ürettiği.
- Oyunda hangi spawner'ların olacağı ve kaçıncı seviye item ürettikleri `SpawnerBarConfig`'te. Üretilen seviye spawner'ın kendisinde değil, oyundaki yerleşiminde tutuluyor; aynı spawner farklı bir yerleşimde farklı seviyeden üretebiliyor.
- Şu an 10 zincir, her birinde 5 level ve her zincir için bir spawner var. Sprite'lar 32×32 piksel ve 32 PPU; bir item tam bir hücre (1 birim) kaplıyor. Bütün sprite'lar tek bir atlasta (bkz. Performans).

**Yeni bir zincir eklemek (kod değişikliği yok):**

1. Sprite'ları `Assets/Art` altında zincire ait yeni bir klasöre koy ve klasörü `GameAtlas`'ın listesine ekle.
2. `Create > Game > Item Chain` ile asset oluştur; benzersiz bir `Id` ver, level'ları sırayla ekle, her birine sprite ve merge skoru ver.
3. Zinciri `ItemCatalog`'taki `Chains` listesine ekle; kayıttan yüklenirken zincir burada aranıyor.
4. `Create > Game > Spawner` ile zinciri üreten bir spawner oluştur.
5. Yeni spawner'ı `SpawnerBarConfig`'teki `Spawners` listesine, üreteceği seviyeyle birlikte ekle.

## Grid

- Grid boyutu (varsayılan 5×5), hücre boyutu ve tahta kenarlığının (`GridCorner`) boşluğu `GridConfig`'te; koda sabit yazılmıyor. Boyut grid'in kendi ayarı; grid'i kuran ve hücre pozisyonlarını hesaplayan sistem onu kendi config'inden okuyor.
- Grid bir servis: `GridController`, Gameplay sahnesindeki `GameplayServiceLocator` prefab'ında duruyor ve kendini `IGrid` olarak kaydediyor.
- Grid'i oluşturmak ile grid'le ilgili soruları cevaplamak ayrı sorumluluklar. Hücreleri ve kenarlığı `GridCreator` oluşturuyor; akış yine `GridController`'dan başlıyor, açılışta creator'a config'i verip grid'i kurduruyor. Creator kimseye açılmıyor; grid'i sadece controller kurabiliyor.
- Hücreler ve kenarlık prefab'dan oluşuyor. Kenarlık 9-slice bir sprite; kendi script'i (`GridCorner`) renderer'ını tutuyor ve creator ona sadece boyutunu veriyor, böylece her grid boyutunda bozulmadan uzuyor.
- Hücreler tahtanın altında local pozisyonlarıyla diziliyor; tahta kendi merkezine göre kurulduğu için grid boyutu tek veya çift olsun ortada kalıyor.
- `IGrid` sadece kullanılan sorguları açıyor: tahtanın kapladığı alan (kamera ve spawner sırası), hücre ↔ dünya pozisyonu dönüşümü (yerleştirme ve sürükleme), bir hücrenin tahtanın içinde olup olmadığı ve grid'in boyutu (kayıtta tahtayı dolaşmak). Hücre boyutunu veya tahtanın sahnedeki yerini kimse bilmiyor; ikisi de değişse grid'i kullanan kod değişmiyor.
- Hücre pozisyonu ve tahta alanı hesabı (`GridMath`) `internal`; dışarıdan herkesin çağırabileceği bir yardımcı sınıf olsaydı her çağıran grid boyutunu ve hücre boyutunu kendisi vermek zorunda kalırdı. Creator ve controller aynı alan hesabını kullandığı için kenarlık ile kameranın sığdırdığı alan hep aynı. Ayrı bir sınıfta durmasının sebebi saf fonksiyon olarak test edilebilmesi.
- Pozisyondan hücreye çevirirken `Mathf.RoundToInt` kullanmadım; tam yarıda en yakın çift sayıya yuvarladığı için hücre sınırlarında bir hücre bir yöne, diğeri öbür yöne kayardı. `FloorToInt` her sınırda aynı davranıyor.

### Hangi hücrede ne var

- Grid'in durumu tek bir yerde, `GridObjectHolder`'da tutuluyor; hangi hücrede ne olduğunun tek doğru kaynağı bu, ekrandaki objeler bu durumu sadece gösteriyor. Diğer sistemler ona `IGridObjectHolder` üzerinden erişiyor.
- Hücrede duran objelerin tipi `IGridObject`; şu an bunu sadece item uyguluyor. Holder'ı generic (`GridObjectHolder<T>`) da yazabilirdim, ama hücrede durabilecek her şeyin ortak bir sözleşmesi olması yetiyor; generic parametre başka bir işe yaramazdı.
- `IGridObject` içi boş bir sözleşme; bu yüzden item sistemi holder'dan aldığı objenin item olup olmadığını tip kontrolüyle (`is Item`) anlıyor. Holder'ı doğrudan `Item` tutacak şekilde yazsaydım bu kontrole gerek kalmazdı; ama grid sistemi item sistemine bağımlı olurdu ve hücreye item dışında bir şey koymak grid'i değiştirmeyi gerektirirdi. Bağımlılığın yönünü korumak için tip kontrolünü bilerek kabul ettim.
- Bir hücrenin üç durumu var: boş, kiralanmış ve sahipli. Bir item'ın gideceği hücre, item oraya varana kadar kiralanıyor; kiralanmış hücre başka bir üretimde ya da bırakmada hedef olamıyor ve `Get` henüz sahibini döndürmüyor. Item hücreye varınca sahibi oluyor (`Occupy`), hücreden ayrılınca hücre boşalıyor (`Vacate`). Üretim ve boş hücreye taşıma aynı kuralı kullanıyor; yoldaki bir item ne başka bir item'la aynı hücreye düşebiliyor ne de varmadan merge'e hedef olabiliyor.
- Merge sonucu oluşan item ise kiralamadan hemen hücrenin sahibi oluyor. Kiralamanın sebebi item'ın hücreye gitmesi; merge sonucu hiçbir yere gitmiyor, olduğu hücrede büyüyerek beliriyor. Büyüme animasyonunu beklemek, oyuncunun yeni item'ı hemen bir sonraki merge için sürüklemesini gereksiz yere engelliyordu.
- Kiralamanın sebebi item'ların hücreye animasyonla gitmesi; item'ın hücreyi seçtiği an ile hücreye vardığı an arasında zaman var. Alternatifler:
  - Hücreyi item yola çıkarken doğrudan sahiplendirebilirdim. Ama o zaman yoldaki item merge'e hedef olurdu; oyuncu henüz varmamış bir item'ın üstüne bırakınca uçmakta olan item yarıda silinirdi.
  - Hücreyi sadece item varınca işaretleyebilirdim. Ama peş peşe dokunuşlarda iki item aynı boş hücreyi seçebilir, tahta dolu olduğu halde boş sanılıp item üretilebilirdi.
  - Animasyon süresince girdiyi kilitleyebilirdim. Ama oyuncu her üretimden sonra beklemek zorunda kalırdı; peş peşe dokunarak hızlı üretmek mümkün olmazdı.
  
  Kiralama üçünü de çözüyor: hücre seçildiği an başkasına kapanıyor, ama içindeki item varana kadar kimse ona dokunamıyor ve oyuncu hiç beklemiyor.
- Kiralanmamış bir hücreyi sahiplenmek, boş olmayan bir hücreyi kiralamak veya sahibi olmayan bir hücreyi boşaltmak hata fırlatıyor. Sessizce yok sayılsaydı grid ile ekran birbirinden ayrışır ve hata çok sonra ortaya çıkardı.
- Holder diğer servisler gibi `GameplayServiceLocator`'da duruyor ve kendini kaydediyor; hücre dizilerini `Awake`'te `GridConfig`'teki boyutla oluşturuyor (sebebi Kayıt bölümünde). Grid'in sorgularını cevaplayan `GridController`'dan ayrı; biri tahtanın geometrisini, diğeri içindekileri biliyor.
- Hangi objenin hangi hücrede olduğunu sadece holder biliyor (`CellOf`). Item hücresini ayrıca tutabilirdi; başta öyleydi ve holder ile item'ı birlikte güncellemek için `Place`/`Unplace` ikisini aynı anda değiştiriyordu. Ama aynı bilgi iki yerde durunca biri unutulduğunda grid ile item birbirinden kopabiliyordu. Şimdi item'ın `Cell`'i holder'a soruluyor; bilginin tek bir sahibi var. Item'ın hücreye yerleşmesi (`Place`) ve ayrılması (`Unplace`) holder'ı güncelliyor ve collider'ını açıp kapatıyor.

## Kamera

- Kamera orthographic; oyun 2D sprite'larla XY düzleminde kuruluyor.
- Kamerayı sabit bir boyutta da bırakabilirdim. Ama grid boyutu config'ten değişebiliyor ve telefonların en-boy oranları çok farklı. `CameraFitter` açılışta kamerayı tahtanın ortasına alıyor ve tahtanın tamamı, kenarlarda `CameraConfig`'teki boşlukla, ekrana sığacak şekilde boyutunu ayarlıyor.
- Kamera sadece tahtayı sığdırıyor, altındaki spawner sırasını hesaba katmıyor. Oyun dikey; telefonlarda tahta genişliğe göre sığdığı için altında spawner'lara yer kalıyor.
- Kamera tahtanın alanını `IGrid`'den alıyor; grid'in nasıl kurulduğunu bilmiyor. Alan config'ten hesaplandığı için kameranın grid kurulumunu beklemesi gerekmiyor.

## Item üretimi

- Spawner'lar tahtanın altında tek sıra halinde duruyor. Başta tahtanın hücrelerinde duruyorlardı ve sürüklenip taşınabileceklerdi; vazgeçtim. Hücre kaplamadıkları için tahtada item'lara yer kalıyor ve spawner'larla item'lar aynı kuralları paylaşmak zorunda kalmıyor.
- Spawner sırası grid'e benzer şekilde kuruluyor: `SpawnerBarController` açılışta sıranın yerini hesaplayıp `SpawnerBarCreator`'a spawner'ları oluşturtuyor. Başka bir sistem ondan bir şey istemediği için servis değil; Gameplay sahnesinde kendi prefab'ıyla duruyor. Hangi spawner'ların olacağı, ürettikleri seviye, boyutları, aralarındaki mesafe ve tahtayla arasındaki boşluk `SpawnerBarConfig`'te. Sıra tahtanın alanına göre konumlanıyor; grid boyutu değişince kendiliğinden altında kalıyor.
- Dokunma için Lean Touch kullanıyorum; dokunma ve mouse farkını kendisi soyutluyor. `TapController` parmak basıldığı anda altında bir `ITappable` obje varsa ona haber veriyor; neye dokunulduğunu bilmiyor.
- Lean Touch'ın hazır tap event'i, basıp bırakma 0.2 saniyeden kısa sürmezse tetiklenmiyor; normal bir tıklama çoğu zaman bundan uzun sürdüğü için tıklamalar kaçıyordu. Bırakmayı beklemek de tepkiyi geciktiriyordu. Spawner basıldığı anda üretiyor ve kısa bir ölçek animasyonuyla (`SpawnerConfig`) dokunuşa karşılık veriyor; basılı tutup parmağı kaydırarak iptal etmek mümkün değil.
- Peş peşe her basış bir item üretiyor; animasyon üretimi beklemiyor. Yeni basışta önceki animasyon tamamlanmış sayılıp ölçek tabana dönüyor ve yeni animasyon oradan başlıyor; animasyonlar üst üste binip spawner'ı büyütmüyor. Animasyon sadece büyüyüp geri dönüyor, tabanın altına inmiyor; inseydi collider da küçülür ve hızlı basışlarda kenara gelen dokunuşlar kaçardı.
- Spawner'lar tahtanın hücrelerinde durmadığı için dokunulan obje 2D collider ile bulunuyor. Sonucu collider döndüğü için `ITappable` collider'ın objesinden `TryGetComponent` ile alınıyor; bu sadece basma ve bırakma anında çalışıyor, her karede değil.
- Spawner'a dokununca spawner önce rastgele bir boş hücre kiralıyor, sonra kendi zincirinden, config'te verilen seviyedeki item'ı `ItemCreator`'a ürettiriyor.
- Item spawner'ın üst kenarında küçük doğuyor, zıplayarak ve büyüyerek hedef hücreye gidiyor; varınca hücrenin sahibi oluyor. Süre, zıplama yüksekliği, ease ve başlangıç ölçeği `ItemAnimationConfig`'te. Spawner'ın ortasında doğsaydı ilk anı spawner'ın görselinin üstünde kaybolur, item geç çıkmış gibi görünürdü.
- Item yoldayken yok olursa (örneğin oyuncu o sırada ana menüye dönerse) animasyon iptal ediliyor ve item hücreye yerleşmiyor. Tween item'ın yok olma iptaline bağlı; iptal edilen tween'in beklenmesi de iptal oluyor, tamamlanmış sayılmıyor.
- Item'ı oluşturup hücreye götürmek `ItemCreator`'ın işi; spawner item prefab'ını, animasyonu ve hücreye nasıl yerleştiğini bilmiyor. Merge sonucu oluşan item'lar da aynı yoldan oluşuyor.
- Boş hücre yoksa item üretilmiyor ve "Tahta dolu!" uyarısı gösteriliyor. Uyarı metni `SpawnerConfig`'te.
- Uyarılar kısa bir toast olarak gösteriliyor. `ToastService` diğer servisler gibi `GameplayServiceLocator`'da duruyor; ekrandaki görünüm (`ToastView`) HUD canvas'ında ve servise sahnede referans olarak veriliyor. Görünüm servis olarak kaydolsaydı servis olmayan bir obje servis listesine girerdi. Açılma ve görünme süreleri `ToastConfig`'te.
- Toast tahtanın ortasında çıkıyor. Tahtanın ekrandaki yerini dünya pozisyonundan hesaplayabilirdim; ama kamera zaten tahtayı ekranın ortasına aldığı için toast'u ekranın ortasına sabitlemek yetiyor. Toast safe area'nın değil canvas'ın altında; çentikli ekranlarda safe area'nın ortası ekranın ortasından kayıyor.

## Sürükleme ve merge

- `DragController` parmak basınca altındaki `IDraggable`'ı collider ile buluyor, parmak hareket ettikçe ve bırakınca ona haber veriyor; neyi sürüklediğini bilmiyor. Sürüklemeyi item sınıfına özel de yazabilirdim; ama sürükleme bir etkileşim, sürüklenen şeyin kendisi değil. İleride item dışında sürüklenebilecek bir şey eklenirse `DragController` değişmiyor.
- Item'ın collider'ı sadece bir hücrenin sahibiyken açık. Yolda olan ya da yeni hücresine oturmakta olan bir item sürüklenemiyor; yarım kalmış bir yerleşmenin üstüne ikinci bir sürükleme binmiyor.
- Sürüklenen item sürükleme boyunca kendi hücresinin sahibi olarak kalıyor. O sırada spawner'dan çıkan bir item onun hücresine düşemiyor ve bırakma başarısız olursa item'ın döneceği yer hep boş.
- Sürükleme ve bırakmayı item değil `ItemDragHandler` yönetiyor. Bırakılan hücreye bakıp (`Locate`) karar veriyor: hedef boşsa item oraya taşınıyor; birleşebildiği bir item'sa merge oluyor; birleşemediği bir item'sa geri dönüp sallanıyor, max seviyedeyse "Maksimum seviye!" uyarısı da çıkıyor (`ItemDragConfig`); tahta dışı, kendi hücresi ya da kiralanmış hücrede sadece geri dönüyor. Sürüklerken hedef vurgusu da aynı hücre kontrolünü kullanıyor.
- Merge kuralı (`MergeRules`) saf bir fonksiyon ve sonucu bir enum: birleşir, farklı zincir, farklı seviye veya max seviye. Sonucu `bool` olarak da döndürebilirdim; ama oyuncuya sadece max seviyede uyarı gösteriliyor ve bunun için birleşmemenin sebebini bilmek gerekiyor.
- Bu işlerin hepsi başta `ItemMerger` adlı tek bir sınıftaydı; ama adı merge diyen sınıf vurgu, taşıma ve geri dönmeyi de yapıyordu. Şimdi her biri yaptığı işin adını taşıyor: `ItemMerger` sadece merge gerçekten olacaksa çalışıyor, `ItemHighlighter` sadece vurguları (hedef ve adaylar) gösteriyor, `ItemDragHandler` akışı yönetiyor. Highlighter ve merger servis değil; handler'la aynı objede duruyor ve handler'ın işini görüyorlar.
- Item'lar handler'ı sürükleme sırasında `IItemDragHandler` üzerinden buluyor. Başta merge'ü yapan sınıfı `ItemCreator` item'ları oluştururken item'lara veriyordu; ama merge de creator'ı kullandığından ikisi birbirine bağlıydı. Handler servis olunca bağımlılık tek yöne indi: merger creator'ı kullanıyor, creator merger'ı hiç bilmiyor.
- `IItemDragHandler` diğer interface'lerin aksine `internal`. Metotları item sistemine ait `Item` tipini alıyor ve başka bir sistemin ona erişmesi gerekmiyor; public olsaydı item'ın iç tipini dışarı açmak gerekirdi.
- Sürüklenen item en üstte çiziliyor; sürükleme sırasındaki sorting order `ItemDragConfig`'te, bırakınca prefab'daki değere dönüyor. Sürüklemeye başlarken bir artırıp bırakınca bir azaltmak da mümkündü; ama bir bırakma kaçarsa item kalıcı olarak yanlış katmanda kalırdı.
- Sürüklerken parmağın altında bir item varsa onun hücresi vurgulanıyor: birleşebiliyorsa yeşil, birleşemiyorsa kırmızı. Boş hücreler vurgulanmıyor; oraya bırakmak her zaman taşımak demek ve oyuncunun bilmesi gereken bir şey değil. Her hücreye ayrı bir vurgu durumu tutmak yerine tek bir vurgu objesi hedef hücrenin üstüne taşınıyor (`CellHighlighter`). Renkler grid'in yerleşiminden ayrı, `CellHighlighterConfig`'te. Vurgu grid sisteminde olduğu için merge'ü bilmiyor; hedefin geçerli olup olmadığını (`isValid`) alıyor, renkleri de `ValidColor`/`InvalidColor`. Merge'ün ne demek olduğunu item sistemi biliyor.
- Sürükleme başlayınca, sürüklenen item'la birleşebilecek item'ların hücreleri sarı işaretleniyor (`ICellHighlighter.ShowCandidates`); hedef vurgusu bu işaretlerin üstünde çiziliyor. Adaylar sürüklemenin başında bir kez bulunuyor. Her karede yeniden bulabilirdim; ama sürükleme sırasında tahtadan item eksilmiyor, sadece spawner'dan yenisi gelebiliyor. Bunun için her kare tahtayı dolaşmaya değmez; sürükleme sırasında tahtaya oturan bir item o sürüklemede işaretlenmiyor.
- Birleşmeyen bir item'ın üstüne bırakılınca (farklı zincir, farklı seviye, max seviye) item hücresine dönüp kısa bir süre yatayda sallanıyor. Boş alana, tahta dışına ya da kiralanmış hücreye bırakmada sadece geri dönüyor; orada oyuncunun yanlış yaptığı bir şey yok. Sallanma, ölçek animasyonları gibi görselin alt objesinde; collider ve hareket tween'i etkilenmiyor. Süre, güç ve titreşim `ItemAnimationConfig`'te.
- Item'lar object pool'dan geliyor; Unity'nin hazır `UnityEngine.Pool.ObjectPool<T>`'si kullanılıyor. `ItemCreator` item'ı pool'dan alıyor, merge'de silinen item'lar `Destroy` yerine pool'a geri dönüyor; yeni bir item oluşturmak gerektiğinde önce pool'daki pasif item'lar kullanılıyor. Pool'u kendim de yazabilirdim; ama Unity'nin pool'u hazır ve test edilmiş, editörde aynı item'ın iki kez geri verilmesini de yakalıyor.
- Pool'a dönen item sıfırlanıyor: çalışan tween'leri durduruluyor (görselin büyüme ve sallanma tween'leri sona getirilerek, böylece yarıda kalan bir animasyon bir sonraki kullanımda kaymış ya da küçük bir görsel bırakmıyor), collider'ı kapanıyor ve obje pasif oluyor. Pozisyon, ölçek ve görsel item pool'dan tekrar alınınca baştan veriliyor. Pool'a sadece bir hücrenin sahibi olan item'lar dönebiliyor (merge); yolda olan bir item'ın collider'ı kapalı olduğu için merge'e hedef olamıyor, bu yüzden bekleyen bir yerleşme hiçbir zaman yarıda kalıp pool'daki bir item'a uygulanmıyor.
- Merge sonucu oluşan item hedef hücrede küçük başlayıp büyüyerek beliriyor; taşınan ya da geri dönen item hücresine kısa bir tween ile kayıyor. Süreler ve ease'ler `ItemAnimationConfig`'te.
- Bir item sürüklenmeye başlayınca sadece hareket tween'i (taşınma ya da geri dönme) durduruluyor; büyüme animasyonu devam ediyor. Bütün tween'leri durdursaydım, büyümesi bitmeden sürüklenen yeni item yarım boyda kalırdı.
- Item iki parçaya ayrılmış: `Item` durumu (zincir, level), yerleşmeyi ve sürükleme girdisini; `ItemView` görünüşü (sprite, sorting order) ve bütün animasyonları (doğma, taşınma, merge, sallanma) tutuyor. Hepsi tek sınıftaydı; ama bir item'ın ne olduğu ile nasıl göründüğü ayrı sebeplerle değişiyor. Animasyonları çağıranlar (`ItemCreator`, `ItemDragHandler`) `item.View` üzerinden çağırıyor.
- Büyüme ve küçülme animasyonları item'ın kendisine değil, görselin durduğu alt objeye (`Visual`) uygulanıyor; collider kökte kaldığı için hep hücre boyunda. Ölçeği kökte animasyonlasaydım collider da küçülür, merge'den hemen sonra item'ı kenarından tutmaya çalışan dokunuşlar boşa giderdi.

## Skor

- Her başarılı merge, oluşan item'ın level'ındaki `MergeScore` kadar puan kazandırıyor; puan değerleri zincir verisinde, koda yazılmıyor. Seviye yükseldikçe puan artıyor (şu an L2 = 10, L3 = 25, L4 = 50, L5 = 100).
- Merge olunca `ItemMerger` bir `ItemMergedEvent` fırlatıyor (merge'ün olduğu pozisyon ve kazanılan puan). Skoru doğrudan çağırabilirdim; ama merge'ün olması skordan bağımsız bir bildirim. Item sistemi skordan haberdar değil; skoru ve efekti ilgilendiren her sistem aynı event'i kendisi dinliyor.
- Gerçek skor kayıtta duruyor; `ScoreController` (`IScore`) onu merge olduğu anda artırıp kaydediyor. Ekrandaki sayı ise merge'ün olduğu yerden üstteki skor barına uçan yıldız vardığında artıyor (`ScoreStarFlight`, `ScoreView`). Skorun kendisini de yıldız varınca artırabilirdim; ama o zaman yıldız yoldayken oyundan çıkılırsa puan kaybolurdu. Gerçek değer anında doğru, sadece gösterimi animasyonlu.
- Skor barı HUD'da, safe area içinde. Barı da spawner sırası gibi dünyada kurabilirdim; ama UI'da olunca çentikli ekranlara uyuyor ve kameranın tahtayı sığdırma hesabına girmiyor. Uçan yıldız, merge'ün dünya pozisyonu ekran pozisyonuna çevrilerek UI'da oluşuyor.
- Uçan yıldızlar da item'lar gibi `ObjectPool`'dan geliyor. Uçuş süresi, ease ve ikonun büyüme animasyonu `ScoreConfig`'te.
- Skor barındaki yıldız görseli diğer görsellerle aynı kurallarla çizildi: 32×32 piksel, koyu kontur, sol üstte açık ve sağ altta koyu ton.

## Kayıt

- Skor ve tahtadaki item'lar tek bir `player.json` dosyasında. Dosyayı `PlayerDataStore` (`IPlayerProgress`) okuyup yazıyor; proje boyunca yaşayan servislerle birlikte Splash'teki `ProjectServiceLocator`'da duruyor ve dosyayı açılışta bir kez okuyor. Her sistem kendi dosyasını yazabilirdi; ama kayıt Gameplay açılmadan hazır olunca, skoru ve tahtayı kuran sistemler "kayıt okundu mu" diye `Start` sırasına güvenmek zorunda kalmıyor.
- Store sadece okuyup yazıyor, oyun kuralı bilmiyor. Tahtadaki bir item'ın kaydı (`BoardItemData`) düz veri: zincirin id'si, level ve hücre. Kayıt sistemi item ve skor sistemlerini tanımıyor; bağımlılık onlardan kayda doğru.
- Dosya JSON; Unity'nin `JsonUtility`'siyle yazılıyor (`Core/Utilities/JsonFileUtility`). PlayerPrefs da kullanılabilirdi; ama tahta bir liste ve PlayerPrefs liste tutmuyor, elle string'e çevirmek gerekirdi.
- Dosya önce geçici bir dosyaya yazılıyor, sonra eskisinin yerine konuyor. Doğrudan üstüne yazsaydım, yazma sırasında uygulama kapanırsa yarım bir dosya kalır ve bütün kayıt kaybolurdu; şimdi ya eski ya yeni kayıt duruyor. Okunamayan (bozuk) bir dosya hata olarak loglanıyor ve oyun boş bir kayıtla açılıyor; ilk kayıtta dosya düzeliyor.
- Zincir kayda asset adıyla ya da listedeki sırasıyla değil, kendi `Id`'siyle yazılıyor; yüklenirken `ItemCatalog` id'den zinciri buluyor. Asset adı yeniden adlandırınca, sıra da liste değişince değişir ve eski kayıtlar bozulurdu.
- Tahta, bir item hücresine her oturduğunda kaydediliyor: item `Place` olunca `ItemPlacedEvent` atıyor, `ItemBoardSaver` tahtayı dolaşıp kaydı yeniliyor. Sadece uygulama durunca ya da kapanırken kaydetmek de mümkündü; ama o anda yolda olan (doğan ya da taşınan) bir item hiçbir hücrede olmadığı için kayıttan düşerdi, Android de uygulamayı haber vermeden kapatabiliyor. Yerleşme anında kaydedince yarım kalmış bir durum hiç yazılmıyor. Tahtada en fazla 25 item var; her hamlede yazmanın maliyeti yok.
- Her kayıtta tahtanın tamamı holder'dan baştan okunuyor. Sadece son işlemi (merge'de silinen iki item ve yeni item, taşımada eski ve yeni hücre) kayda işleyebilirdim; ama maliyetin asıl kısmı tarama değil, dosyanın tamamının yeniden yazılması ve o her iki yolda da aynı. Değişiklik bazlı güncellemede kayıttaki liste tahtanın ikinci bir kopyası olurdu; bir akış onu güncellemeyi unutursa kayıt sessizce gerçek tahtadan sapar, hata da ancak oyun yeniden açılınca görünürdü. Tahtanın tamamını okuyunca kayıt hep holder'ın anlık görüntüsü; bir sapma olsa bile bir sonraki yerleşmede kendiliğinden düzeliyor. Kayıt bir gün ağırlaşırsa iyileştirilecek yer dosya yazma olur (değişiklikleri biriktirip aralıklarla ve uygulama arka plana geçerken yazmak).
- Yerleşme event ile duyuruluyor, çünkü item kaydı bilmiyor. Üretim, taşıma ve merge'ün hepsi aynı `Place`'ten geçtiği için kaydedenin her akışa ayrıca çağrı eklemesi gerekmiyor.
- Skor, arttığı anda kaydediliyor.
- Gameplay açılınca kayıttaki item'lar animasyonsuz, doğrudan hücrelerine yerleştiriliyor. Bilinmeyen bir zincir id'si, zincirde olmayan bir level ya da tahtanın dışında kalan bir hücre (örneğin grid küçültüldüyse) atlanıyor; oyun açılmaya devam ediyor. Kaydetme yükleme bittikten sonra başlıyor; yüklenen item'ların yerleşmesi dosyayı tekrar yazmıyor.
- Yükleme `Start`'ta item yerleştirdiği için holder'ın hücre dizileri ve item pool'u `Awake`'te kuruluyor; aksi halde `Start`'ların sırasına kalırdı. İkisi de sadece kendi ayarını kullanıyor, başka bir servise erişmiyor; "`Awake`'te başka servise erişilmez" kuralı korunuyor.
- Gameplay'de sağ üstteki buton oyunu sıfırlıyor; önce bir onay popup'ı soruyor (`ResetPopupView`). Onaylanınca kayıt silinip Gameplay sahnesi yeniden yükleniyor. Tahtayı ve skoru yerinde temizlemek de mümkündü; ama her sistem için ayrı bir "temizle" yolu yazmak ve ekrandaki skoru ayrıca güncellemek gerekirdi. Sahneyi yeniden yüklemek her şeyi zaten var olan oyuna giriş yolundan sıfırlıyor.
- Sahne yüklenmeye başlayınca (`SceneLoadStartedEvent`) tahta kaydedilmeyi bırakıyor. Sıfırlama onaylandığı anda yolda bir item varsa, sahne kapanmadan yerine oturup silinen kaydı eski tahtayla geri yazmasın diye.
- Popup'ları `PopupService` açıyor; Toast gibi `GameplayServiceLocator`'da duruyor. Popup'ların açıldığı katman ve arkadaki karartma HUD'da; servise sahnede referans olarak veriliyor.
- Editörde `MergeMechanic > Reset Player Data` menüsü kaydı siliyor; play mode dışında kullanılabiliyor.

## Performans

Ölçümler 25 item'lık dolu bir tahtada, profiler sayaçlarıyla (Batches, Draw Calls, SetPass Calls) yapıldı.

| Durum | Draw call | SetPass |
|---|---|---|
| Ayrı dokular, SRP Batcher açık | 59 | 4 |
| Atlas, SRP Batcher açık | 56 | 3 |
| Atlas, SRP Batcher kapalı, Dynamic Batching açık | **3** | **3** |

- **Sprite Atlas:** Bütün sprite'lar (item, spawner, hücre, vurgu, kenarlık ve UI) tek bir Sprite Atlas V2'de (`Art/GameAtlas`): 512×256 tek sayfa, sıkıştırmasız, point filter, mipmap yok. Dünya ve UI aynı atlası paylaşıyor; tahta kenarlığı ile UI çerçeveleri aynı sprite'ı kullandığı için ayrı atlaslar bu sprite'ı ikiye bölerdi. Rotation ve tight packing kapalı; UI `Image`'ları döndürülmüş ya da sıkı paketlenmiş sprite'ları desteklemiyor. Font atlası ayrı (TMP).
- **Batching:** URP'de SRP Batcher sprite'ları tek çizimde birleştirmiyor, sadece çizimler arasındaki durum değişimini ucuzlatıyor; atlas olsa da her sprite ayrı bir draw call kalıyordu (56). SRP Batcher kapatılıp Dynamic Batching açılınca aynı materyal ve aynı dokudaki sprite'lar birleşiyor: tahtadaki 54 sprite tek çizimde, toplam 3 draw call. Bu da ancak atlasla mümkün; ayrı dokularla her doku değişimi birleşmeyi bozardı. SRP Batcher'ın avantajı çok sayıda farklı materyal ve shader'da; bu sahnede tek bir sprite materyali ve UI var. Dynamic batching her karede sprite köşelerini CPU'da dönüştürüyor, sprite'lar birkaç köşeden oluştuğu için bu maliyet küçük. SRP Batcher açık bırakılsaydı da bu ölçekte sorun olmazdı; ama draw call sayısı tahtadaki item sayısıyla büyüyordu, şimdi sabit.
- **Render ayarları:** URP mobil asset'inde HDR, MSAA, gölgeler, post-processing, depth ve opaque texture kapalı; 2D sprite oyununda kullanılmıyorlar.
- **Kod:** Oyun kodunda kare başı çalışan `Update` yok; her şey girdi ve event'le tetikleniyor. Item'lar ve uçan yıldızlar pool'dan geliyor, event'ler allocation'sız struct. Hedef kare hızı cihaz gücüne göre 60 ya da 30 (`FrameRateConfig`).

## Tasarım kalıpları

| Kalıp | Nerede | Neden | Alternatif |
|---|---|---|---|
| Service Locator | `ServiceLocator`, `ISceneLoader` | Servislere interface üzerinden, kurulum maliyeti olmadan erişim | Zenject gibi bir DI container; bu boyutta öğrenme ve açılış maliyeti fazla |
| Singleton | `MonoSingleton` (`ServiceLocator`) | Proje boyunca tek bir locator, sahne geçişlerinde yaşıyor | Saf statik sınıf; ama o zaman Splash'teki servislerle birlikte bir prefab'da duramazdı |
| Observer (Event Bus) | `EventBus`, `Game.Events` | Atan ve dinleyen birbirini tanımıyor (loading ekranı ve sahne yükleme; merge ile skor ve uçan yıldız; item yerleşmesi ile kayıt) | C# event'leri; dinleyenin atan sınıfa referans tutması gerekirdi |
| Type Object | `ItemChainData`, `SpawnerData` | Item ve spawner türleri sınıf değil veri; yeni tür kodsuz ekleniyor | Her item türü için alt sınıf ya da enum + switch; her yeni tür kod değişikliği |
| Factory | `ItemCreator`, `SpawnerBarCreator`, `GridCreator` | Oluşturma tek yerde; prefab'ı, animasyonu ve yerleşmeyi çağıran taraf bilmiyor | Her çağıranın kendisi `Instantiate` etmesi; aynı kurulum kodu birkaç yerde tekrar ederdi |
| Object Pool | `ItemCreator`, `ScoreStarFlight` (`UnityEngine.Pool.ObjectPool<T>`) | Merge'de silinen item'lar ve uçan yıldızlar yeniden kullanılıyor; oyun boyunca sürekli `Instantiate`/`Destroy` ve bunun getirdiği GC yükü oluşmuyor | Her seferinde `Instantiate`/`Destroy`; tahtada en fazla grid boyutu kadar item olduğu için maliyeti küçük olurdu, ama oyun uzadıkça birikiyor. Pool'u elle yazmak; Unity'nin hazır pool'u yeterli |

## Bilinen kısıtlamalar

- Kamera sadece tahtayı sığdırıyor. 3:4 gibi geniş ekranlarda (tablet) tahta yüksekliğe göre sığdığı için altındaki spawner sırası kısmen ekran dışında kalabilir.

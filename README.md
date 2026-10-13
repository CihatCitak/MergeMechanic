# MergeMechanic

Grid tabanlı bir merge oyununun temel mekanikleri.

- Unity 6000.0.62f1, URP
- UniTask, DOTween, Lean Touch

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
- Spawner'lar da ScriptableObject (`SpawnerData`): generator görseli ve hangi zincirden ürettiği.
- Oyunda hangi spawner'ların olacağı ve kaçıncı seviye item ürettikleri `SpawnerBarConfig`'te. Üretilen seviye spawner'ın kendisinde değil, oyundaki yerleşiminde tutuluyor; aynı spawner farklı bir yerleşimde farklı seviyeden üretebiliyor.
- Şu an 10 zincir, her birinde 5 level ve her zincir için bir spawner var. Sprite'lar 256 piksel ve 256 PPU; bir item tam bir hücre (1 birim) kaplıyor.

**Yeni bir zincir eklemek (kod değişikliği yok):**

1. `Create > Game > Item Chain` ile asset oluştur; level'ları sırayla ekle, her birine sprite ve merge skoru ver.
2. Zinciri mevcut bir spawner'ın `Chains` listesine ekle ya da `Create > Game > Spawner` ile yeni bir spawner oluştur.
3. Yeni spawner'ı `SpawnerBarConfig`'teki `Spawners` listesine, üreteceği seviyeyle birlikte ekle.

## Grid

- Grid boyutu (varsayılan 5×5), hücre boyutu ve tahta kenarlığının (`GridCorner`) boşluğu `GridConfig`'te; koda sabit yazılmıyor. Boyut grid'in kendi ayarı; grid'i kuran ve hücre pozisyonlarını hesaplayan sistem onu kendi config'inden okuyor.
- Grid bir servis: `GridController`, Gameplay sahnesindeki `GameplayServiceLocator` prefab'ında duruyor ve kendini `IGrid` olarak kaydediyor.
- Grid'i oluşturmak ile grid'le ilgili soruları cevaplamak ayrı sorumluluklar. Hücreleri ve kenarlığı `GridCreator` oluşturuyor; akış yine `GridController`'dan başlıyor, açılışta creator'a config'i verip grid'i kurduruyor. Creator kimseye açılmıyor; grid'i sadece controller kurabiliyor.
- Hücreler ve kenarlık prefab'dan oluşuyor. Kenarlık 9-slice bir sprite; kendi script'i (`GridCorner`) renderer'ını tutuyor ve creator ona sadece boyutunu veriyor, böylece her grid boyutunda bozulmadan uzuyor.
- Hücreler tahtanın altında local pozisyonlarıyla diziliyor; tahta kendi merkezine göre kurulduğu için grid boyutu tek veya çift olsun ortada kalıyor.
- `IGrid` sadece kullanılan sorguları açıyor: tahtanın kapladığı alan (kamera ve spawner sırası) ve bir hücrenin dünya pozisyonu (item yerleştirme). Hücre boyutunu veya tahtanın sahnedeki yerini kimse bilmiyor; ikisi de değişse grid'i kullanan kod değişmiyor.
- Hücre pozisyonu ve tahta alanı hesabı (`GridMath`) `internal`; dışarıdan herkesin çağırabileceği bir yardımcı sınıf olsaydı her çağıran grid boyutunu ve hücre boyutunu kendisi vermek zorunda kalırdı. Creator ve controller aynı alan hesabını kullandığı için kenarlık ile kameranın sığdırdığı alan hep aynı. Ayrı bir sınıfta durmasının sebebi saf fonksiyon olarak test edilebilmesi.

### Hangi hücrede ne var

- Grid'in durumu tek bir yerde, `GridObjectHolder`'da tutuluyor; hangi hücrede ne olduğunun tek doğru kaynağı bu, ekrandaki objeler bu durumu sadece gösteriyor. Diğer sistemler ona `IGridObjectHolder` üzerinden erişiyor.
- Hücrede duran objelerin tipi `IGridObject`; şu an bunu sadece item uyguluyor. Holder'ı generic (`GridObjectHolder<T>`) da yazabilirdim, ama hücrede durabilecek her şeyin ortak bir sözleşmesi olması yetiyor; generic parametre başka bir işe yaramazdı.
- Bir hücrenin üç durumu var: boş, kiralanmış ve sahipli. Üretilen item spawner'dan hedef hücreye uçarken o hücreyi kiralıyor; kiralanmış hücre başka bir üretimde seçilmiyor. Item hücreye varınca sahibi oluyor (`Occupy`). Böylece yoldaki bir item ne başka bir item'la aynı hücreye düşebiliyor ne de varmadan merge'e hedef olabiliyor.
- Kiralanmamış bir hücreyi sahiplenmek hata fırlatıyor. Sessizce yok sayılsaydı grid ile ekran birbirinden ayrışır ve hata çok sonra ortaya çıkardı.
- Holder diğer servisler gibi `GameplayServiceLocator`'da duruyor ve kendini kaydediyor; hücre dizilerini açılışta `GridConfig`'teki boyutla oluşturuyor. Grid'in sorgularını cevaplayan `GridController`'dan ayrı; biri tahtanın geometrisini, diğeri içindekileri biliyor.

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
- Item'ı oluşturup hücreye götürmek `ItemCreator`'ın işi; spawner item prefab'ını, animasyonu ve hücreye nasıl yerleştiğini bilmiyor. Merge sonucu oluşacak item'lar da aynı yoldan oluşacak.
- Boş hücre yoksa item üretilmiyor ve "Tahta dolu!" uyarısı gösteriliyor. Uyarı metni `SpawnerConfig`'te.
- Uyarılar kısa bir toast olarak gösteriliyor. `ToastService` diğer servisler gibi `GameplayServiceLocator`'da duruyor; ekrandaki görünüm (`ToastView`) HUD canvas'ında ve servise sahnede referans olarak veriliyor. Görünüm servis olarak kaydolsaydı servis olmayan bir obje servis listesine girerdi. Açılma ve görünme süreleri `ToastConfig`'te.
- Toast tahtanın ortasında çıkıyor. Tahtanın ekrandaki yerini dünya pozisyonundan hesaplayabilirdim; ama kamera zaten tahtayı ekranın ortasına aldığı için toast'u ekranın ortasına sabitlemek yetiyor. Toast safe area'nın değil canvas'ın altında; çentikli ekranlarda safe area'nın ortası ekranın ortasından kayıyor.

## Tasarım kalıpları

| Kalıp | Nerede | Neden | Alternatif |
|---|---|---|---|
| Service Locator | `ServiceLocator`, `ISceneLoader` | Servislere interface üzerinden, kurulum maliyeti olmadan erişim | Zenject gibi bir DI container; bu boyutta öğrenme ve açılış maliyeti fazla |
| Singleton | `MonoSingleton` (`ServiceLocator`) | Proje boyunca tek bir locator, sahne geçişlerinde yaşıyor | Saf statik sınıf; ama o zaman Splash'teki servislerle birlikte bir prefab'da duramazdı |
| Observer (Event Bus) | `EventBus`, `Game.Events` | Atan ve dinleyen birbirini tanımıyor (loading ekranı, sahne yükleme) | C# event'leri; dinleyenin atan sınıfa referans tutması gerekirdi |
| Type Object | `ItemChainData`, `SpawnerData` | Item ve spawner türleri sınıf değil veri; yeni tür kodsuz ekleniyor | Her item türü için alt sınıf ya da enum + switch; her yeni tür kod değişikliği |
| Factory | `ItemCreator`, `SpawnerBarCreator`, `GridCreator` | Oluşturma tek yerde; prefab'ı, animasyonu ve yerleşmeyi çağıran taraf bilmiyor | Her çağıranın kendisi `Instantiate` etmesi; aynı kurulum kodu birkaç yerde tekrar ederdi |

## Bilinen kısıtlamalar

- Kamera sadece tahtayı sığdırıyor. 3:4 gibi geniş ekranlarda (tablet) tahta yüksekliğe göre sığdığı için altındaki spawner sırası kısmen ekran dışında kalabilir.

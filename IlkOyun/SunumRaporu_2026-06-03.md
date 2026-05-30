# Soul Hunter Sunum Raporu

Sunum tarihi: 3 Haziran 2026
Proje türü: 2D platform + puzzle + mini-boss + turn-based battle prototype
Geliştirme ortamı: Visual Studio, C#, .NET 9, MonoGame

## 1. Projenin Son Hali

Oyunun güncel yapısı tek sahnelik lineer ilerleyişten çıkarılmış ve büyük bir overworld haritasına dönüştürülmüştür.

Akış şu şekildedir:

- Oyuncu büyük platform haritasında ilerler.
- Haritada Mario benzeri sağ-sol gezen mini enemy'ler vardır.
- Oyuncu bu enemy'leri üzerlerine zıplayarak yok edebilir.
- Bu enemy'lerden birinden `Double Jump` modulu dusurulebilir.
- Haritada 3 adet rune switch tabanlı puzzle vardır.
- Her puzzle ilgili mini-boss kapısının bariyerini açar.
- 3 farklı kapı, 3 farklı mini-boss savaşına açılır.
- Mini-bosslardan element almak için sadece yenmek yetmez, `Catch` ile yakalamak gerekir.
- `Fire`, `Water`, `Wind` elementleri toplandıktan sonra final kapı açılır.
- Son kapı final boss savaşına götürür.
- Basarili boss tamamlamalarından sonra ayni harita devam eder ama tema değişir ve zorluk artar.

Bu yapı sayesinde oyun artık:

- world map progression içeriyor
- puzzle + exploration içeriyor
- platform ve battle sistemini aynı çatı altında topluyor
- oyuncuya sıra ve amaç duygusu veriyor

## 2. Oyun Döngüsü

Sunumda oyunun ana döngüsünü şu cümleyle anlatabilirsin:

`Explore -> switch puzzle çöz -> kapıyı aç -> mini-boss'a gir -> catch ile elementi al -> 3 elementi tamamla -> final gate -> final boss`

Bu döngü projenin hem tasarımını hem de kod mimarisini açıklamak için çok güçlüdür.

Yeni sürümde buna bir ek halka daha vardır:

`boss tamamlanır -> tema değişir -> zorluk artar -> aynı map yeni cycle olarak devam eder`

## 3. Hangi Ders Konularını Kullandık

## 3.1 Inheritance

Projede `Game1` sınıfı, MonoGame kütüphanesindeki `Game` sınıfından kalıtım alır.

Bu sayede şu yaşam döngüsü metodlarını override ettik:

- `Initialize`
- `LoadContent`
- `Update`
- `Draw`

Neden kullandık:
- oyun döngüsünü yönetmek için
- framework mantığını doğrudan C# kodu üzerinden göstermek için

Kod referansı:
- [Game1.cs](<C:\Users\isu\Documents\New project\IlkOyun\Game1.cs:8>)

## 3.2 Generics ve Collections

Projede birden fazla generic koleksiyon kullandık:

- `List<Rectangle>`: platformlar ve çarpışma yüzeyleri
- `List<PuzzleSwitch>`: puzzle anahtarları
- `List<PuzzleBarrier>`: kapı bariyerleri
- `List<DoorDefinition>`: boss kapıları
- `List<PatrollingEnemy>`: overworld enemy listesi
- `HashSet<ElementType>`: capture edilmiş elementler

Neden kullandık:
- sayı olarak değişebilen oyun objelerini esnek yönetmek için
- harita ve progression mantığını tekil değişkenler yerine koleksiyonlarla kurmak için

Kod referansı:
- [Game1.cs](<C:\Users\isu\Documents\New project\IlkOyun\Game1.cs:23>)

## 3.3 Enum Kullanımı

Projede birden fazla enum kullandık:

- `GamePhase`
- `ElementType`
- `DoorType`

Neden kullandık:
- oyunun durumunu açık şekilde yönetmek için
- string karşılaştırmaları yerine daha güvenli ve okunabilir bir yapı kurmak için

Örnek:
- `Overworld`
- `Battle`
- `Victory`
- `Defeat`

Kod referansı:
- [GameModels.cs](<C:\Users\isu\Documents\New project\IlkOyun\GameModels.cs:5>)

## 3.4 Sınıf Tasarımı ve Veri Modelleme

Projede verileri ayrı sınıflarla modelledik:

- `EnemyDefinition`
- `DoorDefinition`
- `PuzzleSwitch`
- `PuzzleBarrier`
- `PatrollingEnemy`
- `BattleState`

Neden kullandık:
- her nesnenin tek sorumluluğu olsun diye
- büyük harita yapısını daha okunabilir hale getirmek için
- mini-boss, final boss, kapı ve battle verisini birbirine karıştırmamak için

Bu karar özellikle best practice açısından önemli.

Kod referansı:
- [GameModels.cs](<C:\Users\isu\Documents\New project\IlkOyun\GameModels.cs:23>)

## 3.5 State Yönetimi

Projede oyun akışı küçük bir state machine mantığıyla yönetildi.

Ana state'ler:

- `Overworld`
- `Battle`
- `Victory`
- `Defeat`

Neden kullandık:
- platform, sonuç ekranı ve savaş ekranını tek yerde kontrol etmek için
- kodun dallanıp karışmasını önlemek için

Kod referansı:
- [Game1.cs](<C:\Users\isu\Documents\New project\IlkOyun\Game1.cs:89>)

## 3.6 Collision ve Oyun Mantığı

Projede çarpışma mantığı yoğun kullanıldı:

- oyuncu ve platform çarpışması
- oyuncu ve puzzle switch çarpışması
- oyuncu ve kapı çarpışması
- oyuncu ve mini enemy çarpışması
- oyuncu düşmanın üstüne gelirse enemy ölür
- stomp edilen enemy'den upgrade drop çıkabilir

Mario benzeri enemy mantığı burada kuruldu:

- üstten temas varsa enemy ölür
- yandan temas varsa oyuncu checkpoint'e döner
- kill sonrası `Double Jump` modulu düşebilir

Neden kullandık:
- platform oyunu hissini vermek için
- overworld bölümünü sadece yürüyüş alanı olmaktan çıkarmak için

Kod referansı:
- [Game1.cs](<C:\Users\isu\Documents\New project\IlkOyun\Game1.cs:333>)

## 3.7 Hareket Akıcılığı

Zıplama sisteminde iki küçük oyun hissi tekniği kullanıldı:

- jump buffer
- coyote time

Neden kullandık:
- oyuncu tam platform kenarında veya iniş anında tuşa bastığında input boşa gitmesin diye
- hareket daha akıcı hissettirsin diye

Buna ek olarak:

- `Double Jump` bir eklenti gibi tasarlandı
- oyuncu bunu haritadaki stomp edilen enemy drop'undan topluyor

Bu sayede hem platforming rahatladı hem de map içinde küçük bir progression katmanı oluştu.

## 3.8 Puzzle Sistemi

Puzzle mantığı rune switch ve barrier yapısıyla kuruldu.

Sistem:

- oyuncu ilgili rune switch'e temas eder
- switch aktif olur
- ilgili mini-boss kapısının bariyeri kalkar
- oyuncu artık kapıya erişebilir

Neden kullandık:
- map progression hissi vermek için
- haritaya amaçlı keşif eklemek için

Kod referansı:
- [Game1.cs](<C:\Users\isu\Documents\New project\IlkOyun\Game1.cs:318>)
- [GameModels.cs](<C:\Users\isu\Documents\New project\IlkOyun\GameModels.cs:91>)

## 3.9 Battle Sistemi

Turn-based savaş sistemi korunmuştur.

Komutlar:

- `Attack`
- `Guard`
- `Catch`
- `Fire`
- `Water`
- `Wind`

Buradaki önemli değişiklik:

- summon yetenekleri artık yerde toplanan itemlerden değil
- mini-bossları `Catch` ile yakalayarak kazanılan elementlerden geliyor

Yani battle sistemi overworld progression ile doğrudan bağlı hale getirildi.

Kod referansı:
- [Game1.cs](<C:\Users\isu\Documents\New project\IlkOyun\Game1.cs:468>)

## 3.10 Tema ve Zorluk Dönüşü

Projede boss tamamlandıktan sonra dünya tamamen resetlenmek yerine aynı harita üzerinden yeni bir cycle başlatılır.

Bu cycle sisteminde:

- son tamamlanan bossun temasına göre harita renklenir
- map enemy hızları artar
- battle enemy HP ve damage değerleri yükselir

Neden kullandık:
- tekrar oynanabilirlik eklemek için
- "aynı map ama daha zor" hissi oluşturmak için
- boss başarılarını oyun dünyasında görsel olarak hissettirmek için

## 3.11 Robustness ve Hata Kontrolü

Projede temel güvenlik ve dayanıklılık kontrolleri var:

- oyuncu haritadan düşerse checkpoint'e döner
- HP değerleri alt ve üst sınıra bağlanır
- catch chance üst limite sabitlenir
- kapı kilidi koşulları kontrol edilir
- mini-boss sadece catch ile tamamlanır

Bu, oyunun bozulmasını engeller ve daha stabil sunum yapılmasını sağlar.

Kod referansı:
- [Game1.cs](<C:\Users\isu\Documents\New project\IlkOyun\Game1.cs:289>)
- [Game1.cs](<C:\Users\isu\Documents\New project\IlkOyun\Game1.cs:720>)

## 3.12 Kamera ve Büyük Harita Yönetimi

Bu sürümde önceki prototipten farklı olarak kamera takibi eklendi.

Neden kullandık:
- tek ekrana sığmayan büyük bir map hissi vermek için
- 4 kapılı world structure kurabilmek için

Kod referansı:
- [Game1.cs](<C:\Users\isu\Documents\New project\IlkOyun\Game1.cs:587>)

## 4. Bilerek Kullanmadığımız Konular

Her derste gördüğümüz konuyu zorla eklemedik. Sunumda bunu dürüstçe söylemek daha doğru olur.

## 4.1 Async / Await / Threading

Bu projede aktif olarak kullanılmadı.

Sebep:
- oyun döngüsü senkron ve frame tabanlı ilerliyor
- mevcut prototype için zorunlu değildi

Kullanılabileceği yerler:
- save/load
- asset yükleme
- arka plan veri işlemleri

## 4.2 Reflection ve Attributes

Kullanılmadı.

Sebep:
- bu oyunun mevcut ölçeğinde gereksiz soyutlama oluştururdu

## 4.3 File Operations / Serialization

Henüz kayıt sistemi yok.

Sebep:
- öncelik oynanabilir çekirdek tasarımı kurmaktı

İleride eklenebilir:
- boss kapısı ilerleme kaydı
- capture edilen elementlerin saklanması
- checkpoint sistemi

## 4.4 Interface Kullanımı

Bu sürümde interface tabanlı bir AI sistemi kurulmadı.

Sebep:
- ödev ölçeği için veri modeli + state tabanlı çözüm daha sade tutuldu

İleride eklenebilir:
- `IEnemyBehavior`
- `IBattleAction`
- `IInteractable`

## 5. Sunumda Ne Anlatmalısın

En temiz akış şu olur:

1. Oyun fikrini anlat
   Büyük harita, puzzle switchler, 3 mini-boss kapısı, final gate

2. Oynanış döngüsünü anlat
   keşif -> puzzle -> kapı -> savaş -> catch -> final boss

3. Kod mimarisini anlat
   `Game1.cs` ana akış
   `GameModels.cs` veri modelleri

4. Ders konularıyla bağ kur
   inheritance, collections, enum, state management, collision, robustness

5. Bilerek kullanmadığın konuları açık söyle
   async, reflection, serialization, interface

6. Son olarak oyunun genişletme potansiyelini söyle
   save system, gerçek sprite assetleri, interface tabanlı enemy AI, stage artışı

## 6. Hoca Ne Sorabilir

## Soru 1
Neden bu yapıda büyük bir map kullandınız?

Cevap:
Çünkü proje tek savaşlık bir demo gibi kalmasın istedik. Harita, puzzle ve boss kapıları ekleyerek progression hissi oluşturduk.

## Soru 2
Puzzle sistemi nasıl çalışıyor?

Cevap:
Her mini-boss kapısı bir rune switch ile ilişkilendirildi. Switch aktif olunca ilgili bariyer kalkıyor ve kapıya erişim açılıyor.

## Soru 3
Mini-boss'u yenmek neden yetmiyor?

Cevap:
Çünkü tasarım gereği final kapının açılması için element essence gerekiyor. O essence sadece `Catch` ile alınıyor. Bu da battle sistemine stratejik bir amaç katıyor.

## Soru 4
Mario benzeri enemy mantığını nasıl kurdunuz?

Cevap:
Oyuncunun önceki ve mevcut pozisyonunu karşılaştırdık. Yukarıdan düşerken temas varsa enemy öldü, yandan temas varsa oyuncu checkpoint'e döndü.

## Soru 5
Final kapı neden doğrudan açık değil?

Cevap:
Çünkü map progression mantığı kurmak istedik. 3 mini-boss'tan elde edilen Fire, Water ve Wind elementleri final kapının açılma koşulu oldu.

## Soru 6
Double jump'ı neden baştan açık yapmadınız?

Cevap:
Onu küçük bir map upgrade'i gibi düşündük. Hem platforming sorununun çözümü oldu hem de overworld enemy'leri sadece engel değil, ödül kaynağına dönüştürdü.

## Soru 7
Burada hangi veri yapıları en önemli rolü oynuyor?

Cevap:
`List<>` koleksiyonları ve `HashSet<ElementType>`. Özellikle capture edilen elementleri tekrar etmeyecek şekilde tutmak için `HashSet` mantıklı oldu.

## Soru 8
Neden enum kullandınız?

Cevap:
Hem oyun fazlarını hem de element tiplerini açık, güvenli ve okunabilir şekilde yönetmek için.

## Soru 9
Eğer projeyi büyütseydiniz ilk neyi değiştirirdiniz?

Cevap:
Overworld, battle ve enemy davranışlarını ayrı sınıflara bölüp interface tabanlı bir yapı kurardım. Save/load sistemi de eklerdim.

## Soru 10
Neden MonoGame kullandınız?

Cevap:
Çünkü hazır editör bağımlılığı yerine C# ders konularını doğrudan kod üstünde göstermek istedik. MonoGame bu açıdan daha öğretici oldu.

## Soru 11
Bu projede design pattern var mı?

Cevap:
Resmi isimlendirilmiş ağır bir pattern kurmadık ama state machine mantığı kullandık. Ayrıca veri modelini ayırarak maintainable bir yapı hedefledik.

## 7. Kısa Savunma Cümleleri

Sunumda kullanabileceğin güvenli cümleler:

- "Bu projede önceliğimiz tam çalışan küçük ama anlamlı bir oyun döngüsü üretmekti."
- "Yeni sürümde lineer yapıyı bırakıp world map progression ekledik."
- "Puzzle ve battle sistemlerini doğrudan progression ile bağladık."
- "Özellikle inheritance, collections, enum tabanlı state yönetimi ve collision mantığını aktif kullandık."
- "Her konuyu zorla eklemek yerine proje mantığına gerçekten katkı veren yapıları kullandık."

## 8. Sonuç

Bu proje şu an:

- büyük harita hissi veren bir overworld içeriyor
- puzzle tabanlı kapı açma sistemi içeriyor
- Mario benzeri mini enemy yapısı içeriyor
- 3 mini-boss ve 1 final boss progression sistemi içeriyor
- catch ile açılan final gate mantığı içeriyor
- ders konularının önemli bölümünü pratik olarak gösteriyor

Sunumdaki en güçlü noktan şu:
Bu proje sadece fikir anlatmıyor; oynanış akışı, progression mantığı ve kod mimarisi olan çalışan bir prototype sunuyor.

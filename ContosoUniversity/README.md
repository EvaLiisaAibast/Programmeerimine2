# Harjutus: ASP.NET Core MVC with EF Core – tutorial series

Iseseisev läbimine Microsofti õpetusest **"ASP.NET Core MVC with EF Core – tutorial series"**
(Contoso University näidisrakendus, 10 osa).

- Õpetus: <https://learn.microsoft.com/aspnet/core/data/ef-mvc/intro>
- Läbitud: **10 / 10 õpetust**
- Kaust: `C:\Users\aeval\Desktop\AspNetCore-MVC-EFCore-Tutorial\`
- Projekt: `ContosoUniversity` (ASP.NET Core MVC + EF Core + SQL Server LocalDB)

## Kuidas käivitada

```bash
cd C:\Users\aeval\Desktop\AspNetCore-MVC-EFCore-Tutorial\ContosoUniversity
dotnet run
```

Või ava `ContosoUniversity.slnx` Visual Studios ja vajuta F5.

- Andmebaas luuakse ja täidetakse näidisandmetega automaatselt käivitamisel
  (`Data/DbInitializer.cs` rakendab kõik ootavad migratsioonid ja seab andmed sisse).
- Ühendus string: `appsettings.json` → `ConnectionStrings:DefaultConnection`
  (`Server=(localdb)\mssqllocaldb;Database=ContosoUniversity;...`).
- Kui soovid andmebaasist puhtalt alustada:

```bash
cd ContosoUniversity
dotnet ef database drop
dotnet ef database update
dotnet run          # istutab andmed uuesti
```

EF CLI on paigaldatud **kohaliku tööriistana** (`dotnet-tools.json` kausta juures),
seega `dotnet ef ...` töötab otse. (Õpetuses kasutatakse `dotnet tool install --global dotnet-ef`.)

## Migratsioonid (loodud õpetuste järjekorras)

| # | Migratsioon | Õpetus | Mis muutus |
|---|-------------|--------|-----------|
| 1 | `InitialCreate` | 4. Migrations | Course, Student, Enrollment tabelid |
| 2 | `MaxLengthOnNames` | 5. Complex data model | `[StringLength(50)]` nimedele (andmekaoekaotuse hoiatus, nagu õpetuses) |
| 3 | `ColumnFirstName` | 5. Complex data model | `[Column("FirstName")]` → veeru nime muutmine |
| 4 | `ComplexDataModel` | 5. Complex data model | Instructor, Department, OfficeAssignment, CourseAssignment, Course.DepartmentID |
| 5 | `RowVersion` | 8. Concurrency | Department `[Timestamp] byte[] RowVersion` |
| 6 | `Inheritance` | 9. Inheritance | Person tabel + Discriminator (TPH), kohandatud SQL andmete säilitamiseks |

## Õpetuste kaupa – mis tehti

| Õpetus | Tehtu | Peamised failid |
|--------|-------|-----------------|
| 1. Get started | Projekt, andmudel (Student/Enrollment/Course), `SchoolContext`, DI registratsioon, `DbInitializer`, menüü ja avaleht | `Program.cs`, `Data/SchoolContext.cs`, `Data/DbInitializer.cs`, `Models/*.cs`, `Views/Shared/_Layout.cshtml`, `Views/Home/Index.cshtml` |
| 2. CRUD | StudentsController kohandatud CRUD: Details koos `Include/ThenInclude/AsNoTracking`, Create `try-catch` + `Bind`, Edit "loe-esi-ja-uenda" (`TryUpdateModelAsync`), Delete veateade | `Controllers/StudentsController.cs`, `Views/Students/*` |
| 3. Sorting, filtering, paging | Järjestus, otsing, `PaginatedList` (õpetuses 3 lehte lehel, siin **50**), About lehe statistika rühmitusega | `PaginatedList.cs`, `Controllers/StudentsController.cs`, `Controllers/HomeController.cs`, `Views/Home/About.cshtml` |
| 4. Migrations | `EnsureCreated` asendatud migratsioonidega, `InitialCreate` loodud ja rakendatud | `Data/DbInitializer.cs`, `Migrations/` |
| 5. Complex data model | Validatsiooni atribuudid, Instructor/OfficeAssignment/Department/CourseAssignment, komposiitvõti, laiendatud istutusandmed | `Models/Instructor.cs`, `Models/Department.cs`, `Models/OfficeAssignment.cs`, `Models/CourseAssignment.cs`, `Data/SchoolContext.cs` |
| 6. Reading related data | Courses lehe `Include(Department)`, Instructors Index koos `InstructorIndexData` (õpetaja → kursused → tudengid) | `Controllers/CoursesController.cs`, `Controllers/InstructorsController.cs`, `Models/SchoolViewModels/InstructorIndexData.cs`, `Views/Instructors/Index.cshtml` |
| 7. Updating related data | Course loomine/muutmine osakonna `SelectList`-iga, Instructors Edit linnutega kursustele + toa asukoht | `Controllers/InstructorsController.cs`, `Controllers/CoursesController.cs`, `Views/Instructors/Edit.cshtml`, `Views/Instructors/Create.cshtml` |
| 8. Concurrency | `RowVersion`, Departments controller vaadetega, konfliktide käsitlemine (`DbUpdateConcurrencyException`), Index-ist RowVersion eemaldatud | `Controllers/DepartmentsController.cs`, `Views/Departments/*`, `Migrations/*_RowVersion.cs` |
| 9. Inheritance | `Person` baasklass, Student/Instructor pärimine, üks Person tabel, kohandatud `Inheritance` migratsioon andmete säilitamiseks | `Models/Person.cs`, `Data/SchoolContext.cs`, `Migrations/*_Inheritance.cs` |
| 10. Advanced | Raw SQL (`FromSqlRaw`), ADO.NET päring About lehel, `ExecuteSqlRaw` uuendus (`UpdateCourseCredits`), dünaamiline LINQ (`EF.Property`) | `Controllers/DepartmentsController.cs`, `Controllers/HomeController.cs`, `Controllers/CoursesController.cs`, `Controllers/StudentsController.cs`, `Views/Courses/UpdateCourseCredits.cshtml` |

## Kontrollitud (automaatne testimine)

- Kõik lehed vastavad 200: `/`, `/Home/About`, `/Students`, `/Students/Details/84`,
  `/Courses`, `/Instructors`, `/Instructors/Index/15`, `/Departments`, `/Departments/Details/5`
  ning kõik Create/Edit/Delete vaated.
- CRUD: uue tudengi lootsime vormi kaudu, leidsime otsinguga, kustutasime – kadus.
- Sorteerimine/dünaamiline LINQ: `?sortOrder=LastName_desc`, `EnrollmentDate`, `EnrollmentDate_desc` – õige järjestus.
- Konfliktide käsitlemine: sama `RowVersion`-iga korduv POST → leht kuvab
  "The record you attempted to edit was modified by another user..." + "Current value: £350,123.00".
- Raw SQL: `FromSqlRaw` Departments Details; About leht ADO.NET `GROUP BY` päringuga;
  `UPDATE Course SET Credits = Credits * 1` → "Number of rows updated: 7".
- Migratsioon `Inheritance` säilitas andmed: enne 8 tudengit + 5 õpetajat,
  pärast `Person` tabelis 8 `Discriminator='Student'` + 5 `Discriminator='Instructor'`,
  11 seost ja 7 kursust alles; `Enrollment.StudentID` viitab uutele ID-dele.
- Suur istutus (pärast "overboard"-uuringut): **500 tudengit, 50 õpetajat, 40 kursust,
  4 osakonda, ~2000 õpingut**; avalehe šipid 500/40/50/4, tudengite leht 50 rida × 10 lehte,
  otsing (`?searchString=Alonso` → 1 vaste), sorteerimine (`LastName_desc` → Young esimesena)
  ja lehtede vahetus (`?pageNumber=2`) kontrollitud curl-iga.

## Kõrvalekalded õpetusest (teadlikud)

1. **.NET 10, mitte .NET 5/6.** Õpetus hoiatab, et see on kirjutatud .NET 5 jaoks
   (Startup.cs). Siin on minimaalne hostimine ühes `Program.cs`-is; muu
   rakenduskood on sama, mis õpetuses.
2. **`<Nullable>disable</Nullable>`** – õpetuse kood ei kasuta NRT-väljendeid.
3. **Bootstrap 5.3** (malli oma): vaadetes asendati BS3/4 klassid
   (`form-group`→`mb-3`, `control-label`→`form-label`, `btn-default`→`btn-outline-secondary`,
   `col-md-offset-2`→`offset-md-2`, `sr-only`→`visually-hidden`).
4. **Scaffolding** toimub Visual Studio kaudu; siin kirjutati samad
   scaffolding'uga tekkivad kontrollerid ja vaated otse (sh õpetuse kohandused).
5. **`DbInitializer` kasutab `context.Database.Migrate()`** (vastab
   `dotnet ef database update`-ile), et rakendus saaks migratsioonid ise rakendada.
6. **`ComplexDataModel` migratsiooni järel kustutati andmebaas** – nõuab õpetus
   (FK-vea asemel), seejärel istutati andmed uuesti.
7. **`Inheritance` migratsiooni scaffolditud `Up/Down` asendati õpetuse
   kohandatud SQL-iga**, mis säilitab andmed; lisaks parandati kaks veeruoperatsiooni
   (`EnrollmentDate` tuleb *lisada*, `HireDate` tuleb *muuta* nullable-iks).
8. **Näidisandmed on "overboard"** (kasutaja soov, nagu MvcMovie 500 filmiga):
   `DbInitializer` istutab **500 tudengit, 50 õpetajat, 40 kursust ja ~2000 õpingut**
   (fikseeritud `Random(4242)` → alati sama andmestik). Kui tudengeid on alla 500,
   ehitab ta tabelid FK-järjekorras puhtaks ja istutab uuesti (restart ei puhasta kunagi
   täidetud andmebaasi). Tudengite lehe lehekülje suurus on **50** (õpetuses 3).
9. **Rakendusest on eemaldatud „Project source code“ nupp ja jaluse „Official sample code“ link** –
   Microsofti õpetuse lehtedel on valikuline link „Download or view the completed application“,
   aga see ei ole õpetuse kohustuslik osa; kasutaja soovil ei näita rakendus enam näidisprojekti
   allikakoodi linki.

## Kujundus: oma kohandatud CSS + pildid + Google Fonts (teema "Baroque")

Välimus on ehitatud nullist (`ContosoUniversity/wwwroot/css/site.css`) ja see on **teadlikult erinev**
MvcMovie projektist – stiilinäide oli ainult see, et *kõik* peab olema stiilitud.

**Teema "Baroque"** – varasem "Ivy" on kasutaja stiiliviidete (vintage/gooti kollaaž) järgi
ümber tehtud: soe pruun-kuld-kreem palett, tolmune roosa (`--rose`) ja oliiv (`--olive`) aktsendid.
Muutuja `--navy` jäi, aga selle väärtus on nüüd soe pruun (`#3d2717`).

- **Taust** – Wikimedia Commonsi barokk-lõueng (`cathedral-ceiling.jpg`, Karlskirche, Viin)
  tumeda pruuni skrimi + servade vinjeediga (dark academia), fikseeritud (`background-attachment: fixed`)
- **Raamitud pärgamentpaneel** – `body > .container`: kuldses kaksikraamis, pruuni ääris;
  üleval ja all 80px **päris pitsipael** (`lace-frame21.png` – läbipaistev PNG, võetud FOOL
  LOVERSi vaba materjali 囲み枠-21 ülemisest ribast `f-ue.gif`; kohanduv
  `background-size: auto 100%` + `repeat-x`); jalus ülaservas sama
- **Navbar** – kreemikad serif-nupud pruuni paksu äärisega (nagu viite pildil "Home | Terms | …"),
  logol kuldne kast; lingi numbrid oxblood-toonis
- **Ornamentika** – päised `❦`-kaunistustega, `h4` tumedate pruunide tähistena (nagu "DO NOT INT IF"),
  nimekirjade ees `†`-dagrid, tabeliridade roosa hover-toon, fondid palistatud (justify)

- **FOOL LOVERSi pitsiraam (囲み枠-21)** – `Students/Details` lehe sisu on originaalse
  8-osalise raamitehnika sees, mis kopeerib lähtelehe `.box21` paigutuse 1:1:
  `.box-top` (nurgad `kado1/2` + ülemine riban `.u01`), `.box-center` (vertikaalsed
  servad `migi`/`hidari` `repeat-y` + `.box-inner` sisu 23px sisetustega), `.box-bottom`
  (riban `.s01` + nurgad `kado3/4`) – kõik 23×23 võtmetatud PNG-id
  (`wwwroot/images/frame21-*.png`), allikas <https://foollovers.com/mat/t-frame21.html>
  (viide jaluses "Image credits"). Kaasas ka lähtelehe skinid `.box21-b/c/d/e`;
  vaikimisi skin `.box21-a` kasutab Baroque'i paberitooni

**Fondid (Google Fonts, laetakse `_Layout.cshtml`-is) – viis kursiivikut:**
- `Cinzel` – pealkirjad, logi, tabelipäised, nupunooled
- `EB Garamond` – kogu tekst (serif, justified, nagu viidetes)
- `Pinyon Script` (`--hand`) – märkused ja vormi kõrvalkaardid (`.hand-note`, `.rail-card`)
- `Great Vibes` (`--script`) – navinupud, kõik nupud, loenduri-šipid, tabelitegevused
  (Edit/Details/Delete), „Create New“/„Back to list“ lingid, jaluse pealkiri
- `Tangerine` 700 (`--script-2`) – hero- ja lehe-päiste kicker-rida, motto „Lux et Veritas“
- `Pirata One` (`--gothic`) – gotiiksed tähistusribad (`h4`, nagu "DO NOT INT IF"),
  `feature-card` pealkirjad, `dl.row` „CU“ vesimärk
- `Inter` / `Caveat` – tagavarafondid

Navinupud mähivad alla 1200px laiusel ekraanil teisele reale (mitte lehe laiusele),
seetõttu horisontaalset kerimisriba ei teki.

**Pildid (`wwwroot/images/`;lingid jaluses "Image credits" (Wikimedia + FOOL LOVERS)):**
`cathedral-ceiling.jpg` (taust), `parchment-texture.jpg` (pabertuur – UESP, Elder Scrolls Wiki),
`lace-frame21.png` + `frame21-*.png` (8-osaline pitsiraam – FOOL LOVERS, 囲み枠-21),
`lace-doily.png` (pitsipael – MET, Wikimedia Commons; hetkel kasutamata),
`hero-graduation.jpg`, `reading-room.jpg`, `lecture.jpg`, `books.jpg`, `campus.jpg`, `lecture-hall.jpg`.

**Mida on ümber ehitatud (sh Microsofti vaikimisi paigutus):**
- **Navbar** – kleepuv (sticky), kullast "CU" embleem (`.seal`), nummerdatud lingid `01…06`, animeeritud hover-joon
- **Avaleht** – fotoga hero (duotoon-scrim, aeglane zoom), elavad statistika-šipid (500/40/50/4, pärit
  `HomeController.Index` `ViewBag`-ist), 3 piltidega `feature-card`-i, üks `duo-card` (käsitsi märkus),
  kursiivimotto „Lux et Veritas“ pealkirja all
- **Lehe-päised** – igal index-lehel `.page-banner` taustapildi, kuldse ääriku, kuldse "Create"-nupu
  ja loenduri-šipiga; **algne `<p><a>Create New</a></p>` on sinna ümber tõstetud**
- **Tabelid** – navy/kuldne päis, rida hoveril kuldses toonis + kuldne vasakjoon, tegevuslingid
  pill-nuppudena (Students lehel `.row-actions` klastriks), `table-success` roheline esiletõst
- **Vormid (Students)** – `form-rail` (käsitsi märkusega kõrvalkaart) + vorm + **`form-bar`**:
  "← Back to list" vasakul, `Create student`/`Save changes` kuldne nupp paremal
- **Detail/kustuta** – `.confirm-bar`: `Edit student` vasakul, `Cancel`/`Back` paremal, `dl.row` kaardil
  sisse "CU" vesimärk
- **Jalus** – 4 veergu (brand, Explore, tutorial, image credits), kuldne üläär, hover-nihutused
- **Lisaks**: kohandatud kerimisriba, `::selection`, `focus-visible`, `prefers-reduced-motion`, print-stiil

Värvide muutmiseks: `:root` (`--navy`, `--gold`, `--parchment`, `--paper`, …); fondid
`--display`, `--body`, `--hand`, `--script`, `--script-2`, `--gothic`.

> **Käivitus:** kasuta tavalist `dotnet run` (loeb `launchSettings.json` → **Development**).
> `--no-launch-profile`/Production korral ei laadi Razor Scoped CSS bundlit (`/ContosoUniversity.styles.css` 500)
> ja vaated jäävad osaliselt stilimata. Võrgu vajavad Google Fonts + pildid laetakse kohaldikust `wwwroot`.

## Viited

- Õpetus: <https://learn.microsoft.com/aspnet/core/data/ef-mvc/intro>
- EF Core migratsioonid: <https://learn.microsoft.com/ef/core/managing-schemas/migrations/>

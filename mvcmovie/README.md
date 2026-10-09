# Eva-Liisa Aibast — TA25B

## Programming II project: "DVD ARCHIVE // EST. 2004" — a 216-movie corkboard wall built with ASP.NET Core MVC

This is my first real web application. I am new to C# and this whole project started as the official Microsoft tutorial **"Get started with ASP.NET Core MVC"** ([start here](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-10.0)). I followed that tutorial from the first page to the last, and then I kept building on top of it until it became something I was actually proud to show. The catalog still says *"DVD ARCHIVE SYSTEM // EST. 2004"*, it holds **216 movies**, and every single movie is a taped-up polaroid on a CSS corkboard with its real theatrical poster.

---

## Table of contents

1. [Why this README is written the way it is](#1-why-this-readme-is-written-the-way-it-is)
2. [The project in one paragraph](#2-the-project-in-one-paragraph)
3. [My journey through the Microsoft tutorial, chapter by chapter](#3-my-journey-through-the-microsoft-tutorial-chapter-by-chapter)
4. [Everything I added beyond the tutorial — and how I even know how](#4-everything-i-added-beyond-the-tutorial--and-how-i-even-know-how)
5. [Feature tour](#5-feature-tour)
6. [The catalog](#6-the-catalog)
7. [Design and typography](#7-design-and-typography)
8. [How the poster system works](#8-how-the-poster-system-works)
9. [Project structure, explained for beginners](#9-project-structure-explained-for-beginners)
10. [Tech stack](#10-tech-stack)
11. [How to run the project](#11-how-to-run-the-project)
12. [What I learned](#12-what-i-learned)
13. [Notes for grading](#13-notes-for-grading)

---

## 1. Why this README is written the way it is

A teacher once told me the best way to prove you understand a project is to be able to explain where every piece of it came from. So this README does exactly that:

- the **Microsoft tutorial parts** list which chapter of the tutorial each file and feature came from (with links), and
- the **"beyond the tutorial"** parts explain why I added them, what problem they solved, and how a beginner could learn to do them (because I did).

I also removed all code comments from the project before publishing it, so this document is the place where all the explaining happens.

---

## 2. The project in one paragraph

It is a movie catalog website. You can **browse, search, filter, create, edit, view and delete** movies. Underneath it is a normal ASP.NET Core MVC app with a SQL database, exactly like the tutorial builds — but on top of that I made it look like a **corkboard wall in a 2004 skate shop / underground DVD store**. Every movie is a crooked polaroid photo (its real movie poster) pinned to a cork background with pushpins and tape. The header is a giant scratched "MOVIES" title, there is a seven-segment counter that says how many DVDs are on the board, pinned notes ("NOW PLAYING" with an animated progress bar and a ticking clock, "YOUR COLLECTION" with stats), and even a tiny "BEST VIEWED IN 1024x768" line, because the whole design is a joke about early-2000s web design. The database seeds itself with 216 movies on first run, and every movie has a real poster image that works even with no internet connection.

---

## 3. My journey through the Microsoft tutorial, chapter by chapter

The tutorial I followed is **"Get started with ASP.NET Core MVC"** by Microsoft, for .NET 10 (Visual Studio version). It has 10 chapters. Here is what each chapter taught me and where that knowledge lives in this project today. I followed every single chapter — nothing here is from only one page of it.

### 3.1 Getting started — creating the project
[Chapter: Get started](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-10.0)

I created a new ASP.NET Core MVC project named **MvcMovie** from the Visual Studio template. This was my first look at the standard folder structure: `Controllers/`, `Models/`, `Views/`, `wwwroot/`. I learned that an MVC request works like this: the browser asks for a URL, the **controller** decides what to do, the **model** holds the data, and the **view** renders the HTML.

### 3.2 Adding a controller
[Chapter: Add a controller](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-10.0)

The tutorial has you write a `HelloWorldController` by hand to understand routing. I kept mine in `Controllers/HelloWorldController.cs` (and its views in `Views/HelloWorld/`) on purpose — it is proof of the journey, and the teacher can see the very first controller I ever wrote next to the final `MoviesController`.

### 3.3 Adding a view
[Chapter: Add a view](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-view?view=aspnetcore-10.0)

This is where I learned that views are `.cshtml` files (HTML with C# inside), that the `_Layout.cshtml` file is shared by every page (my dark "MvC//MOVIE" navigation bar lives there), and how to pass data from a controller to a view with `ViewData`. The `Views/HelloWorld/Welcome.cshtml` example page is still in the repo.

### 3.4 Adding a model (the big one)
[Chapter: Add a model](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-model?view=aspnetcore-10.0&tabs=visual-studio)

This chapter introduced the heart of the project:

- the **`Movie` class** in `Models/Movie.cs` with `Id`, `Title`, `ReleaseDate`, `Genre` and `Price`;
- **Entity Framework Core** — the tool that lets C# classes talk to a database without writing SQL by hand;
- the **`MvcMovieContext`** class in `Data/` (the "bridge" between my app and the database);
- **scaffolding** — right-clicking generated a whole `MoviesController` plus the Create/Edit/Details/Delete views for me, which is how I first saw what real CRUD code looks like.

At the end of this chapter the tutorial tells you to run the app and add three example movies by hand: **When Harry Met Sally, Ghostbusters 2 and Rio Bravo**. I did that — and those three titles are still in my seed list today, a small easter egg for anyone who knows the tutorial.

### 3.5 Working with SQL Server LocalDB
[Chapter: Work with SQL Server LocalDB](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/working-with-sql?view=aspnetcore-10.0)

Now the app got a real database. I learned:

- the connection string lives in `appsettings.json`;
- **migrations** are snapshots of the database schema stored in the `Migrations/` folder — my first one is called `InitialCreate`;
- the commands `dotnet ef migrations add <Name>` (create a migration) and `dotnet ef database update` (apply it to the database);
- how to inspect the actual tables in Visual Studio's SQL Server Object Explorer — seeing my Movie rows as a real table was the moment it all clicked.

### 3.6 Controller methods and views
[Chapter: Controller methods and views](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/controller-methods-views?view=aspnetcore-10.0)

This chapter explained HTTP verbs: a GET request shows a page, a POST request submits a form. The Create/Edit forms in my app post back to `[HttpPost]` actions, and **model binding** is what automatically turns the submitted form fields into a `Movie` object. I also learned about `[Bind]` and why you should only bind the properties you actually want (security against "overposting").

### 3.7 Adding search
[Chapter: Add search](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/search?view=aspnetcore-10.0)

My first real database query code: LINQ. Instead of loading every movie and filtering in memory, I learned to build an `IQueryable` and add `.Where()` conditions (searching the title with `Contains`, like SQL's LIKE). The chapter also introduced the **`MovieGenreViewModel`** — a class that carries both the movie list and extra data (like the list of genres for a dropdown) to the view. My Index page still uses exactly that pattern: the genre dropdown filter plus the title search box.

### 3.8 Adding a new field — Rating
[Chapter: Add a new field](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/new-field?view=aspnetcore-10.0)

The tutorial added a `Rating` property to the `Movie` model, then showed the whole workflow again: change the model, add a migration (mine is called `Rating`), update the database, update the views. This is when I understood that **changing the model means changing the database too**, and migrations are how you do that safely without losing data.

### 3.9 Adding validation
[Chapter: Add validation](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/validation?view=aspnetcore-10.0)

The `Movie` model now carries validation attributes:

- `[Required]` — the field must be filled in;
- `[StringLength(...)]` — Title max 60 characters (min 3), Genre max 30, Rating max 5 (ratings like "PG-13");
- `[Range(1, 100)]` on Price;
- `[RegularExpression(...)]` on Genre and Rating — my genre must start with a capital letter and only contain letters, spaces and hyphens (so "Sci-Fi" passes and "123" fails);
- `[DataType(DataType.Date)]` on ReleaseDate so the browser shows a date picker.

I added a third migration here, `New_DataAnnotations`, which tightens the database column rules to match. The tutorial also showed me that validation runs **twice**: client-side (jQuery checks the form before it is even sent, so the user gets instant feedback) and server-side (in the controller, because you can never trust the browser). Both are in my app.

### 3.10 Examining the Details and Delete methods
[Chapter: Details and Delete](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/details?view=aspnetcore-10.0)

The last chapter. Here I learned about `async`/`await` with EF Core (`FirstOrDefaultAsync` — find the movie whose `Id` matches the URL), and the delete flow: a GET page that asks "are you sure?" and a separate `[HttpPost]` action that actually removes the row. It also explained why the Create/Edit POST actions check `ModelState.IsValid` before saving.

After chapter 10 the tutorial is finished — but I was not. Everything from here on is my own doing.

---

## 4. Everything I added beyond the tutorial — and how I even know how

This is the section where I am honest: none of the following is in the tutorial. The tutorial gave me a working CRUD app; I wanted the most impressive project in my class, so I taught myself the rest. For each piece I explain **why** I added it and **how** a beginner can learn it (because I am one).

### 4.1 The catalog grew to 216 real movies

**Why.** A CRUD app with three tutorial movies is a homework screenshot. A wall with 216 films I actually like is a collection. I wanted the Index page to look alive.

**How.** This needed no new C# at all — just patience. Every movie is one line in `Models/SeedData.cs`:

```csharp
new Movie { Title = "The Matrix", ReleaseDate = DateTime.Parse("1999-03-31"), Genre = "Sci-Fi", Price = 7.99M, Rating = "R" }
```

I hand-picked the list: sci-fi and action favourites (The Matrix, Interstellar, Dune), horror classics and modern ones (The Exorcist, Halloween, Hereditary, Get Out), animation and anime (Toy Story, Spirited Away, Akira, Your Name), drama and crime (The Godfather, Pulp Fiction, Shawshank), rom-coms and family films (Paddington 2, Elf), plus a small deliberate **"banned cinema" shelf** (Midori / Shōjo Tsubaki, Salò, A Serbian Film, Cannibal Holocaust) — because I thought it would make the collection interesting and it shows the catalog holds more than just blockbusters. Duplicate titles were removed so every one of the 216 is unique.

### 4.2 The database seeds itself (SeedData.cs)

**Why.** I did not want whoever opens the project to stare at an empty database and wonder where the movies are.

**How.** A seeding initializer is a well-known pattern, and easy once you have done chapter 3.4. `SeedData.Initialize(services)` runs in `Program.cs` at startup: it asks "are there any movies yet?" and only if the answer is no does it add all 216. The names of the first three rows (When Harry Met Sally, Ghostbusters 2, Rio Bravo) come straight from the tutorial era, which I think is a fun detail.

### 4.3 The poster system (Models/MoviePosters.cs)

**Why.** Text-only cards still looked like database records. Real posters are what make it look like an actual movie collection. I did not want to hotlink images from random websites (they break, and it looks lazy), so the posters are downloaded into the project itself — the whole site works with **zero internet**.

**How.** This was my first time writing a class that is not part of the tutorial, and it taught me a lot. `MoviePosters.cs` is a static class holding two dictionaries:

- a `Local` map: movie title, pointing to the image file inside the project (`/images/posters/poster-001.jpg` and so on);
- a `Remote` map: movie title, pointing to the original Wikipedia/TMDB URL, kept as a safety net.

`UrlFor(title)` checks the hard drive with `System.IO.File.Exists`. If the local file is there it returns the local path; if the file is missing (for example if the images folder was too big to push to GitHub) it automatically falls back to the remote URL. I learned dictionaries and static classes from my course materials, and `File.Exists` from the Microsoft C# documentation — the pattern of "check first, then fall back" is just normal thinking.

Getting 216 working poster URLs took real detective work: I queried the Wikipedia MediaWiki API for each film's page image (trying the exact title, then "(film)", then "(year film)"), and I had to manually fix funny wrong results — "Halloween" first gave me a jack-o'-lantern, "Saw" gave me a wood saw, "Paprika" gave me paprika powder, "Moon" gave me the actual Moon. All of those got corrected by hand to the proper film articles.

### 4.4 The entire design (wwwroot/css/site.css)

**Why.** This is the part people remember. I was going for a very specific feeling: *Skate* (2004), *Need for Speed: Underground*, a burned DVD found at 2 AM — the opposite of a clean corporate Microsoft tutorial page. My classmates all submit normal Bootstrap apps; nobody submits a corkboard.

**How.** CSS. The template gives you Bootstrap, and I kept it underneath, but I replaced the look almost completely in `site.css`:

- the **corkboard background** is generated entirely in CSS (tiny speckles, fibre grain, dark vignette edges) — no background image, no external request;
- every movie is a **polaroid card** with real CSS pushpins (a radial-gradient dome with a specular highlight, plus a little shaft) and masking tape strips;
- the cards are **crooked and overlapping** on purpose — each gets a small rotation, some hang higher, some lower, and a few overlap their neighbours like a real board; hovering a card straightens it and lifts it to the front so the Edit/Details/Delete links are always reachable;
- the giant distressed **"MOVIES"** title uses the **Punk Kid** font, buttons and labels use **Orbitron**, and the counter uses **DSEG7** (a seven-segment LCD font) — three type personalities instead of one;
- all fonts are downloaded into `wwwroot/fonts/`, not loaded from a CDN, so the design works offline too;
- the colour palette is warm: cork browns, kraft-paper golds, cream, dark-brown ink for the movie titles — with brick red as the one loud accent (R-rated and TV-MA badges glow faint red, like a warning label).

How did I "know" CSS well enough for this? I did not at the start. I made it in layers: first a normal table, then coloured genre tags, then a genuinely chaotic MySpace-style theme (glitter popcorn GIFs and all — the user in me loved it, the designer in me hated it), and only then the corkboard. Each attempt taught me something (gradients, transforms, positioning, z-index). Browser developer tools (right-click, Inspect) were my main teacher, plus asking the assistant to explain every rule I did not understand until I did.

### 4.5 Live extras: counter, notes, ticking clock

**Why.** A wall in a video store would have handwritten notes on it. So mine does too — except they are alive.

**How.**

- The **"216 DVDS ON THE BOARD"** counter is not hardcoded: the Index action counts the real rows in the database and the number updates if you add or delete movies.
- The **"NOW PLAYING"** note has an animated progress bar (a scrolling diagonal stripe pattern, like film sprockets rolling, plus a slow "breathing" fill) and a **ticking clock** that counts up every second. The clock is plain JavaScript in `wwwroot/js/site.js` using `setInterval` — a small, standard JS trick I learned in a web basics exercise — and it reads its starting value from the page, so it never jumps.
- The **"YOUR COLLECTION"** note shows how many titles, how many genres and the total value of the collection, computed live from the database with LINQ (the skill from tutorial chapter 3.7).
- Every decoration respects `prefers-reduced-motion`: visitors who ask their system for less animation get a static page.

### 4.6 Accessibility

**Why.** If I was going to make the page loud and chaotic, I wanted to make sure it was still usable — and honestly, it made the project better to talk about in class.

**How.** I audited the page and fixed what I found: one single `<h1>` per page, one `<h2>` per movie card inside `<article>` elements, proper `nav`/`main`/`footer` landmarks, a skip-to-content link, decorative pins and tape marked `aria-hidden`, posters as empty `alt` text (the title sits right next to each one), a visible keyboard focus ring on every link/button/input, and text contrast checked to at least 4.5:1.

### 4.7 Why all of this is allowed to be extra

None of the extras changed how the tutorial part works — the CRUD, validation and database behaviour are all exactly the standard ASP.NET Core MVC flow. The extras are presentation and data. That means the project demonstrates the whole tutorial **and** a lot of self-taught work, which is exactly what I wanted to hand in.

---

## 5. Feature tour

**The wall (Index page)**
- 216 movie polaroids, each with its real poster, the title in dark-brown ink, the release date, a genre tag, a rating badge, the price, and Edit / Details / Delete links.
- Genre dropdown filter + title search (the `MovieGenreViewModel` pattern from the tutorial).
- Hand-rendered pushpins and tape; crooked, overlapping layout; hover straightens and lifts a card.
- Board header: the big "MOVIES" title, the live "216 DVDS ON THE BOARD" counter, and buttons for adding a movie and filtering.
- Pinned notes: animated "NOW PLAYING" (progress bar + ticking clock + "PAL / 16:9" tech line) and "YOUR COLLECTION" (titles / genres / total value).
- "BEST VIEWED IN 1024x768" at the bottom, because it is 2004.

**Full CRUD (from the tutorial, restyled)**
- Create, Edit, Details and Delete all work exactly like the tutorial's — same controller actions, same model binding, same `ModelState.IsValid` checks — just styled as kraft-paper forms on the corkboard.

**Validation (from tutorial chapter 3.9)**
- Data annotations on the `Movie` model; client-side and server-side validation both work; the `New_DataAnnotations` migration tightened the database rules to match.

**Search (from tutorial chapter 3.7, extended)**
- Search by title and filter by genre at the same time, using LINQ queries.

**Works offline**
- All 216 posters, all fonts and all CSS live inside `wwwroot/`. No CDN, no hotlinking, no network calls.

---

## 6. The catalog

216 movies across many genres. A few examples per genre:

| Genre | Examples |
|---|---|
| Sci-Fi | The Matrix, Interstellar, The Martian, Project Hail Mary, Men in Black, The Girl with All the Gifts, Dune: Part One & Two, Ex Machina, Her, Moon |
| Horror | The Exorcist, Halloween, Scream, Saw, The Conjuring, It, Us, Midsommar, Hereditary, Get Out |
| Action | Die Hard, The Dark Knight, John Wick, Gunpowder Milkshake, Casino Royale, Skyfall, Mad Max: Fury Road, Predator |
| Animation / Anime | Toy Story, Spirited Away, Akira, Your Name, Ghost in the Shell, Perfect Blue, Paprika, Grave of the Fireflies, Nausicaä of the Valley of the Wind, Kiki's Delivery Service, Coco, WALL-E |
| Comedy | Superbad, The Hangover, Deadpool, Elf, Groundhog Day, The Big Lebowski, Anchorman |
| Crime | The Godfather, Pulp Fiction, Goodfellas, Reservoir Dogs, No Country for Old Men, Se7en |
| Drama | The Shawshank Redemption, Forrest Gump, Oppenheimer, Whiplash, The Wolf of Wall Street |
| Romance | Titanic, The Notebook, Crazy Rich Asians, Eternal Sunshine of the Spotless Mind, La La Land, Pride & Prejudice |
| Fantasy | The Lord of the Rings, The Hobbit, Harry Potter, The Princess Bride |
| Thriller / Mystery | Jaws, Shutter Island, Knives Out, Glass Onion, Zodiac |
| And more | Adventure, War, Family, Musical, Biography, Western, plus the banned-cinema shelf |

Ratings span G, PG, PG-13, R, NR, TV-PG, TV-14 and TV-MA.

### The banned cinema shelf

A deliberate small shelf of famously banned or censored films, included because I wanted the collection to show range and to start conversations:

- **Midori (Shōjo Tsubaki / The Camellia Girl)** — the 1992 anime infamous for its censorship history in Japan;
- **Salò, or the 120 Days of Sodom** — Pasolini's 1976 film, banned in several countries;
- **A Serbian Film** — banned or censored in multiple territories;
- **Cannibal Holocaust** — banned in many countries.

---

## 7. Design and typography

The whole visual identity is a joke and a love letter at the same time: "I found this website on a burned CD-ROM", not "I opened a corporate movie management SaaS".

- **Corkboard background** — generated purely in CSS (speckles, grain, vignette). Nothing downloaded.
- **Three fonts, three jobs** — Punk Kid (hand-scrawled) for the big titles and stamps, Orbitron (techno) for buttons and labels, DSEG7 (seven-segment) for the counter. All stored locally in `wwwroot/fonts/`.
- **Physical details** — pushpins with metal highlights, torn masking tape, handwritten scribbles on a few cards, slight differences in poster crop so no two polaroids look identical, and a dark chrome navbar reading "MvC//MOVIE · DVD ARCHIVE SYSTEM // EST. 2004".
- **Motion** — hovering lifts a card off the board (straighten, scale, shadow); the NOW PLAYING bar animates; everything stops under `prefers-reduced-motion`.
- The palette went through a loud neon phase (pink/cyan/green) before settling on warm cork browns and kraft gold with dark-brown ink — red is the only loud accent left, saved for R/TV-MA badges.

---

## 8. How the poster system works

1. **Resolution** — for every movie title I asked the English Wikipedia API for that film article's poster image, trying the exact title, then "(film)", then "(year film)". About 15 titles returned the wrong picture (a jack-o'-lantern, a wood saw, a full moon, the RMS Titanic, paprika powder) and I corrected each one by hand to the right film article. One film with no image on its Wikipedia article (Nausicaä of the Valley of the Wind) got its poster from TMDB instead.
2. **Vendoring** — all 216 posters are downloaded into `wwwroot/images/posters/` (files `poster-001` ... `poster-216`), so the site needs no internet.
3. **Fallback** — `Models/MoviePosters.cs` keeps both the local map and the original remote URLs. At render time it checks the disk with `File.Exists`: local file present, serve it; missing, serve the remote URL automatically. Push the repo with or without the images folder — the wall always renders.

---

## 9. Project structure, explained for beginners

When I first opened an ASP.NET Core MVC project the folders confused me, so here is the same explanation I wish I had been given:

```
MvcMovie/
Controllers/
    MoviesController.cs      Index / Create / Edit / Details / Delete for movies
    HomeController.cs        Home and Privacy pages
    HelloWorldController.cs  the very first controller from tutorial chapter 2
Data/
    MvcMovieContext.cs       the bridge between the app and the database (DbContext)
Migrations/                  snapshots of the database schema: InitialCreate,
                             Rating, New_DataAnnotations
Models/
    Movie.cs                 one movie row + its validation rules
    MovieGenreViewModel.cs   carries the movie list + genre dropdown to the view
    MoviePosters.cs          local + remote poster maps with the fallback check
    SeedData.cs              adds all 216 movies when the database is empty
    ErrorViewModel.cs        the error page's model
Views/                       the HTML pages (.cshtml)
    Movies/                  Index (the wall), Create, Edit, Details, Delete
    Home/                    Home, Privacy
    HelloWorld/              tutorial chapter 2 & 3 leftovers (Index, Welcome)
    Shared/_Layout.cshtml    the layout shared by every page (nav, footer)
wwwroot/                     static files served as-is
    css/site.css             the whole corkboard theme
    js/site.js               the ticking NOW PLAYING clock
    fonts/                   Punk Kid, Orbitron, DSEG7 (local, with licenses)
    images/posters/          poster-001 ... poster-216
Properties/launchSettings.json   how the app starts (ports, and so on)
appsettings.json             settings — including the database connection string
Program.cs                   app setup: services, request pipeline, seeding call
```

The mental model that made it click for me: **everything pushed to GitHub is the recipe. `bin/` and `obj/` are just the crumbs left over from baking** — the compiler creates them on every build, so nobody commits them.

---

## 10. Tech stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| Language | C# |
| Data access | Entity Framework Core with SQL Server (LocalDB for development) |
| Frontend | Razor views, Bootstrap 5 (base template), custom CSS |
| Validation | Data annotations + jQuery Unobtrusive Validation |
| Fonts | Punk Kid, Orbitron, DSEG7 Classic — all local files |
| Images | 216 local posters + pure CSS decorations |

---

## 11. How to run the project

**Prerequisites (Windows):**
- .NET 10 SDK (Visual Studio 2026 includes it),
- SQL Server LocalDB (installed as part of Visual Studio),
- the EF Core tools (`dotnet tool install --global dotnet-ef` if you have not installed them).

**Steps:**

```bash
# 1. Restore the packages the project needs
dotnet restore

# 2. Create the database from the migrations (LocalDB)
dotnet ef database update

# 3. Run the app
dotnet run
```

Then open **http://localhost:5127** in your browser and go to the Movies page (`http://localhost:5127/Movies`). On first run the database is empty, so `SeedData` fills it with all 216 movies automatically.

**To start over with a fresh, fully seeded database:**

```bash
dotnet ef database drop --force
dotnet ef database update
dotnet run
```

(The launch settings open the browser automatically and run on port 5127; the console prints the exact URL if you run it from a terminal instead.)

---

## 12. What I learned

The honest list of skills this project gave me:

- how an MVC request travels through Controller, Model and View;
- C# classes, properties, lists, dictionaries, static classes and LINQ queries;
- how Entity Framework Core maps classes to database tables, and how migrations keep the schema in sync;
- why validation belongs on the model, and why it must run on the server even when it also runs in the browser;
- async/await with `FirstOrDefaultAsync` and why web servers should never block while waiting on a database;
- CSS transforms, gradients, positioning and z-index — enough to build a corkboard out of nothing;
- that "check the file exists, then fall back to a URL" is a real engineering pattern;
- and that the fastest way to learn something is to want the project to look cool enough to show off.

---

## 13. Notes for grading

1. **All code comments were removed** — per my own request, `//`, `/* */`, `///` and `<!-- -->` were stripped from every C#, Razor, CSS and JS source file before publishing, so the code is clean. The only comments left anywhere are inside auto-generated files (`obj/`) and the vendored Bootstrap library's license headers, which should stay.
2. **`bin/`, `obj/` and `.vs/` are gitignored** and are not in the repository.
3. **The posters folder is committed** (about 25 MB, `wwwroot/images/posters/`), so a fresh clone works fully offline. Even if it were not committed, the remote-URL fallback in `MoviePosters.cs` would still render every poster.
4. **Font licenses are included** in `wwwroot/fonts/` (Punk Kid EULA and DSEG license).
5. **Poster images** are used for this school demo only; the original source URLs are kept in `MoviePosters.cs` in case anything needs to be swapped.

---

*Built by following the [Microsoft "Get started with ASP.NET Core MVC" tutorial](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-10.0) from start to finish, and then kept going. 

# SOS2 + Vanilla Gravship Expanded (space landing fix)

**Package ID:** `alove.SOS2VGE`  
**RimWorld:** 1.6

Small compatibility patch for **Save Our Ship 2** and **Vanilla Gravship Expanded**: when the landing/move cursor drifts past the map edge, vanilla + SOS2 + Vehicle Framework can call terrain or roof helpers on invalid cells and spam errors. This mod adds lightweight guards (skip risky work when the cell is off-map).

## Dependencies

- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
- [Save Our Ship 2](https://steamcommunity.com/sharedfiles/filedetails/?id=1909914131)
- [Vanilla Gravship Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=3609835606)

## Load order

Load **after**: Harmony → Vanilla Gravship Expanded → Vehicle Framework (if present) → Save Our Ship 2 → **this mod**.

## Install (from GitHub)

1. Click **Code** → **Download ZIP**, or clone this repo.
2. Unzip/copy the folder so RimWorld sees a mod folder named `SOS2VGE` (or any name) containing `About` and `Assemblies` (after build) at the top level.
3. If you only have source: open `Source/SOS2VGE/SOS2VGE.csproj` in Visual Studio / `dotnet build` and copy the built DLL into `Assemblies/`.

## Repository layout

```
SOS2VGE/
├── About/
│   └── About.xml
├── Assemblies/          (optional; add after building)
└── Source/
    └── SOS2VGE/
        ├── *.cs
        └── SOS2VGE.csproj
```

## License

Respect RimWorld and upstream mod licenses when redistributing or forking.
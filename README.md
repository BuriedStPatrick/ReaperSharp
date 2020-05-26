# ReaperSharp
.NET parser client for the REAPER DAW.
Inspired by https://github.com/davemod/UnityReaperParser, extracted core features to a general purpose library.

Very early WIP stuff! This library is very rudimentary and doesn't seem to handle all cases properly. I haven't touched the core parser algorithm other than cleaning up the architecture a bit and fixing code style/indentation.
For instance, will probably crash if you try to parse a REAPER-file with a bunch of binary VST param information. It can't handle tracks with the Kontakt plugin for instance.

I don't know if I'll have time or the patience to keep working on this. So if you want to contribute, feel free to PR, and I'll have a look 👍

## Usage
```C#
using ReaperCore;
using ReaperCore.Extensions;

// Initialize the parser
// DebugLogger: Use whatever ILogger implementation you want. This will get removed at some point.
var parser = new ReaperParser(new DebugLogger()); 
var parseResult = parser.Parse("C:\\path\\to\\file.RPP");

// Find track
var guitarBusTrack = parseResult.FindTrack(t => t.Name == "GTR BUS");

// You can do more stuff, and there's more to be added

```

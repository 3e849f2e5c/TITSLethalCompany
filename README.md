# T.I.T.S Integration for Lethal Company

For regular users view the Thunderstore README [here](PackageSkeleton%2FREADME.md). Rest of this README is technical
stuff.

## Disclaimers
T.I.T.S is paid software by Remasuri3 and must be bought from https://remasuri3.itch.io/tits to be able to use this mod.

The project uses [NativeWebSocket](https://github.com/endel/NativeWebSocket) to connect to the T.I.T.S API websocket.

## Acquiring dependencies to compile the project

Clone the repository and create a folder `libs` and in that folder create `lethalcompany` folder.

Go to your Lethal Company installation and go to `Lethal Company_Data/Managed` and copy every `.dll` file into `libs/lethalcompany` except
 - `netstandard.dll`
 - `mscorlib.dll`
 - Anything that starts with `System.`

Now acquire LC_API from [here](https://github.com/steven4547466/LC-API/releases/latest). Download the `.zip` and copy `BepInEx/plugins/LC_API.dll` into `libs`.

You should then have the following file structure under `libs`:
```
libs/
├─ lethalcompany/
│  ├─ AmazingAssets.TerrainToMesh.dll
│  ├─ Assembly-CSharp.dll
│  ├─ Assembly-CSharp-firstpass.dll
│  ├─ ClientNetworkTransform.dll
│  ├─ DissonanceVoip.dll
│  └─ [...]
└─ LC_API.dll
```

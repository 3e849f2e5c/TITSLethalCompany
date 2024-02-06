# T.I.T.S Integration
https://youtu.be/GBXJCBUrGvs

A mod for Lethal Company
that hooks into [T.I.T.S (Twitch Integrated Throwing System)](https://remasuri3.itch.io/tits) to spawn thrown items
and damage the player.


## Setting up T.I.T.S
If you want items to spawn,
you must import models into T.I.T.S with the exact same name as the item is in Lethal Company.
You can find a collection of some pre-prepared items [here](https://github.com/3e849f2e5c/TITSLethalCompany/releases/download/v1.0.0/LethalCompanyModels.zip)
that are ready to be imported straight into T.I.T.S.

You can import the models by going to "Object Manager"
and then right-clicking the blank space on the left and selecting "Import Model" in the menu.

You can then create a trigger under "Trigger Manager" and under "Object-Customization"
add only the Lethal Company themed items.
Then configure the trigger in a way that you want your viewers to be able to throw flash grenades at you while you're sneaking past dogs.

## Important Notes
1. You must launch T.I.T.S **before** you start a game or connect to a server. If you launch T.I.T.S after loading into a game, you need to exit to the main menu and reconnect or start the server again.
   
2. For best results, everyone on the server needs the mod installed.
    - If you turn off Stun Grenade pin pulling only the host and the player running T.I.T.S need the mod installed.
    - If you also turn off item spawning and only have damage enabled, only the player running T.I.T.S needs the mod regardless if they are the host or not.
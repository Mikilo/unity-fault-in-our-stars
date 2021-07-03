# unity-fault-in-our-stars

# Create a Preset
## From the *Component*'s contextual menu
Add the *Component* *Star* on any *GameObject*.

Click on "Create Preset" from *Star*'s context menu.

## From the shortcut
Select any *GameObject* with a *Component* *Star*.

The default keyboard shortcut is Ctrl + F4.

You can change it at "Edit/Shortcuts...", section "Fault In Our Stars".

It will generate a *Preset* from the first *Star* found.

# Manage the Preset
## Editor Window
Access the window at "Fault In Our Stars/Star Presets".

The top buttons allow you to export the *Presets* into a JSON file,

and load a JSON file into the *Presets*.

From there you can create a new *Preset* from the selection.

There are 3 buttons per Preset.
- **Edit** : Select the *Preset* for inspection.
- **Create** : Drag & drop from this button into the **Scene** to create a *GameObject* from this *Preset*.
- **Delete** : Delete the *Preset*.

## Asset StarPresets
The asset can be find at :
`Assets/Plugins/FaultInOurStars/StarPresets.asset`

Refer to #Manage-the-Preset for usage.

### How it works
The StarPresets asset works by self-containing *Preset* as sub-assets.

Which might bring some uncertainty concerning the reliability with *AssetDatabase*, but so far it seems to work as expected.

Using *Preset* directly greatly eases the process when adding/removing/importing/exporting/dragging. Yes, just that.

# Star
A *Star* in the scene is represent by a colored sphere and its star radius.

The sphere can be manipulated directly from the scene.

Hold **Ctrl** to manipulate the gravity well radius.

# Challenges & Obstacles
I tried to use a maximum of modern/recent APIs. (Preset, SettingsProvider, Shortcut, ScriptableSingleton and a little bit of UIToolkit)

Unfortunately *ScriptableSingleton* failed me, it started to output errors due to the singleton instancing.

I had to use a "normal" singleton pattern.

In the other hand, *Preset* seems pretty solid and reliable. I was the first time I ever used those 2 APIs, I knew them, but never used.

Beside that, not much obstacles on the road, most were previously known from my tooling background hobby.

The UX might not be the best, but there should be enough feedbacks to guide the user.

![alt text](https://img.itch.zone/aW1nLzIxNzU5MTkucG5n/original/88KBuM.png "")
# Open Saber VR

Open Saber VR is an open source clone of the famous and fabolous game Beat Saber. 

I started this project by accident and managed to get the main game logic up and running in 3 days.  Thanks to the open source project beatsaver viewer I was able to get the blocks (notes) in sync with the beat!

Now you would maybe ask yourself what is a Beat Saber clone without any music? Yeah, you are right, it's nothing. But I have some vey good answer to this. Because of the great and wide community of the Beat Saber modders and there custom songs, you can use ANY song from their website and it will work in Open Saber VR. So just go to their websites [BeatSaver](https://beatsaver.com), [BeastSaber](https://bsaber.com) and download any song you want.

At the moment Open Saber VR only supports the notes (beat blocks). Obstacles and mines are not supported but will be added in the future.

If you are interested in helping/contributing to the project (no matter if you are a coding monkey or 3D artist or just have some ideas), feel free to contact me, I will be more than happy to have some help. You can find the complete source code here, so if you want to contribute, just have a look there.


## Updates in this fork (2026 VR pipeline & feature update)

This project originally shipped in 2018 on top of VRTK 3.3.0 and the legacy SteamVR Unity Plugin 1.x, using Unity's old "Virtual Reality Supported" API. That API was removed by Unity in 2020.1, which is why the game only produced a black screen when opened in a modern Unity Editor. This fork brings the whole VR pipeline (and a few gameplay pieces) up to date:

- **VR pipeline**: migrated from VRTK/legacy SteamVR to Unity's official **XR Plugin Management + OpenXR** stack, with the rig built on the **XR Interaction Toolkit** (Starter Assets). This means the game now works with any OpenXR-compatible runtime (SteamVR, Meta Quest Link/Air Link, etc.) instead of being tied to VRTK's SteamVR-only SDK.
- **Post-processing**: fixed shader compatibility issues between the bundled Post Processing Stack v1 and stereo rendering (depth texture sampling wasn't written for single-pass/multi-pass VR rendering).
- **Pause menu**: you can now pause mid-song (primary button on the left controller by default) to resume, restart the current song, or quit back to the main menu, with a confirmation prompt before quitting.
- **Map format**: added support for the **v3 beatmap format** (`colorNotes`) used by most current BeatSaver downloads, on top of the original v2 (`_notes`) parser.
- **Scoring**: added an arcade-style scoring system modeled on Beat Saber's real v2 scoring rules — per-note score (max 115 pts) based on pre-swing angle, post-swing follow-through, and cut precision relative to the block's center, combined with a tiered combo multiplier (1x/2x/4x/8x) that halves (instead of resetting) on a miss. Live score/combo HUD included.
- Removed the now-unused VRTK and legacy SteamVR Unity Plugin assets and packages.

A persistent leaderboard with player profiles is planned but not implemented yet.


## Import songs
Just download any song from BeatSaver or BeastSaber and unzip it to the "OpenSaberVR_data/Playlists" folder. Make sure that each song has its own folder in the Playlists folder. After that run Open Saber VR and the song should be displayed in the menu. Both the classic (`_notes`) and current v3 (`colorNotes`) map formats are supported.


## Gameplay
![alt text](https://img.itch.zone/aW1hZ2UvNDMyMDUzLzIyNDc2OTMucG5n/original/%2Bx5231.png "")
If you know how to play Beat Saber then you are good to go. If not then it's really simple to play, just cut the notes (beat blocks) at the side where the glow bar is with the saber in the same color. The blue saber is the right hand, the red saber is the left hand. The notes will be only sliced if you hit with the correct saber on the correct side. Otherwise the block will just went through.

There is no energy or anything right now, so you can't "loose" a game, the song will play until the end. Score, combo and multiplier are tracked live (see "Updates in this fork" above) — you can pause anytime with the left controller's primary button.

After the song finished, just wait for 5 seconds and you will be pushed back to the main menu where you can select another song.


## Features
 - fully support for the songs from BeastSaber and BeatSaver
 - arcade-style scoring with combo multiplier (see "Updates in this fork")


## Hints
 - Tested with SteamVR and Meta Quest (via Link/Air Link, using SteamVR or the native Oculus OpenXR runtime). Any OpenXR-compatible headset should work.
 - This is an early development version, so expect some bugs 


## Feedback is welcome 
Feedback is very welcome. If you have questions or ideas, then just leave a mail.

## Links
  [itch.io project page](https://devplayrepeat.itch.io/open-saber-vr)

[![GitHub release](https://flat.badgen.net/github/release/FrikandelbroodjeCommunity/SCP-575-LabAPI/)](https://github.com/FrikandelbroodjeCommunity/SCP-575-LabAPI/releases/latest)
[![LabAPI Version](https://flat.badgen.net/static/LabAPI%20Version/v1.1.3)](https://github.com/northwood-studios/LabAPI)
[![Original](https://flat.badgen.net/static/Original/SrLicht?icon=github)](https://github.com/SrLicht/SCP-575)
[![License](https://flat.badgen.net/github/license/FrikandelbroodjeCommunity/SCP-575-LabAPI/)](https://github.com/FrikandelbroodjeCommunity/SCP-575-LabAPI/blob/master/LICENSE)

# About SCP-575

This plugin inserts the SCP-575 in SCP:SL, every round there is a probability that the SCP-575 spawns. SCP-575 can spawn
in LCZ, HCZ and EZ, and will do so accompanied by blackouts. The players that are in the blackout zones are likely to
meet the SCP-575, the SCP-575 will start chasing one of them. If 575 touches them, the players will be devoured, and the
SCP-575 will disappear, for now.

> [!TIP]
> It is possible to make SCP-575 disappear by pointing a flashlight at it or by going to a room where the lights are
> still on.

# Installation

> [!IMPORTANT]
> **Required dependencies:**
> - [FrikanUtils](https://github.com/FrikandelbroodjeCommunity/FrikanUtils/blob/master/FrikanUtils/README.md)
> - [FrikanUtils-Audio](https://github.com/FrikandelbroodjeCommunity/FrikanUtils/blob/master/FrikanUtils-Audio/README.md)

Install the dependencies listed above, and place
the [latest release](https://github.com/gamendegamer321/CameraSystem-LabAPI/releases/latest) in the LabAPI plugin
folder.

# Commands

| command  | Aliases | Usage                     | Required permission | Description                                                                                                                                      |
|----------|---------|---------------------------|---------------------|--------------------------------------------------------------------------------------------------------------------------------------------------|
| `scp575` | `575`   | `575 <player> <duration>` | Remote admin access | Can be used after the round has started to spawn an instance of SCP-575 on a player. This instance will chase the player for the given duration. |

# Config

| Config                                     | Default     | Meaning                                                                                                                                                        |
|--------------------------------------------|-------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `debug_mode`                               | `false`     | When enabled it will show additional debug logs.                                                                                                               |
| `audio_files`                              | ...         | The name of the audio files that can be used as chase music. Each time SCP-575 spawns a random file will be chosen _if_ `scp575.play_sounds` is set to `true`. |
| `spawn_chance`                             | `40`        | The chance of SCP-575 being enabled during a round. 0 = never, 100 = always.                                                                                   |
| `disable_for_scp173`                       | `false`     | When enabled, SCP-575 cannot spawn during a round which has SCP-173.                                                                                           |
| `black_out.active_zones`                   | ...         | The zones where blackouts can occur, this determines the zones where SCP-575 can spawn and roam to.                                                            |
| `black_out.initial_delay`                  | `300`       | The time in seconds between the round start and the first appearance of SCP-575. Only used when `black_out.random_initial_delay` is set to `false`.            |
| `black_out.random_initial_delay`           | `false`     | Whether the initial delay between the round start and the first appearance should be random or deterministic. `true` = random, `false` = deterministic.        |
| `black_out.initial_max_delay`              | `250`       | The maximum amount of delay in seconds between the round start and the first appearance. Only used when `black_out.random_initial_delay` is set to `true`.     |
| `black_out.initial_min_delay`              | `190`       | The minimum amount of delay in seconds between the round start and the first appearance. Only used when `black_out.random_initial_delay` is set to `true`.     |
| `black_out.min_duration`                   | `30`        | The minimum time in seconds SCP-575 will attempt to chase its target before giving up.                                                                         |
| `black_out.max_duration`                   | `90`        | The maximum time in seconds SCP-575 will attempt to chase its target before giving up.                                                                         |
| `black_out.min_delay`                      | `180`       | The minimum amount of time in seconds between appearances of SCP-575.                                                                                          |
| `black_out.max_delay`                      | `400`       | The maximum amount of time in seconds between appearances of SCP-575.                                                                                          |
| `black_out.end_blackout_when_disappearing` | `false`     | Whether, if SCP-575 disappears, the blackout should also end immediately.                                                                                      |
| `black_out.cassie`                         | ...         | The cassie message that is played before the blackout starts.                                                                                                  |
| `black_out.delay_after_cassie`             | `8.5`       | How long, in seconds, there is between the start of the cassie announcement and the blackout.                                                                  |
| `black_out.black_list_rooms`               | ...         | The rooms that will keep their lights on, even if their zone experiences a blackout.                                                                           |
| `scp575.nickname`                          | `SCP-575-B` | The nickname that is given to the SCP-575 dummy.                                                                                                               |
| `scp575.view_range`                        | `12`        | From how far away the nickname of SCP-575 is visible to the player. The default in the base game is `10`.                                                      |
| `scp575.role_type`                         | `Scp106`    | The role SCP-575 is spawned as.                                                                                                                                |
| `scp575.give_ghostly_effect`               | `true`      | Whether the ghostly status effect should be given to SCP-575, this makes it possible for 575 to walk through doors if the `scp575.role_type` is not `Scp106`.  |
| `scp575.kill_reason`                       | ...         | The kill reason shown to the player when they die.                                                                                                             |
| `scp575.play_sounds`                       | `false`     | Whether SCP-575 will play audio while chasing the player. Audio is picked from `audio_files` if enabled.                                                       |
| `scp575.audio_is_looped`                   | `false`     | When `play_sounds` is enabled, this determines whether the audio will loop, or only play once.                                                                 |
| `scp575.sound_volume`                      | `85`        | The volume of the audio played when `play_sounds` is enabled.                                                                                                  |
| `scp575.delay_on_chase`                    | `true`      | When enabled, will case SCP-575 to wait for the time given in `scp575.delay_chase` instead of chasing after the player immediately.                            |
| `scp575.delay_chase`                       | `3.5`       | The time in seconds SCP-575 will wait, if `scp575.delay_on_chase` is enabled.                                                                                  |
| `scp575.max_distance`                      | `28`        | If the distance between SCP-575 and the player they are chasing becomes greater than this, SCP-575 gives up and disappears.                                    |
| `scp575.medium_distance`                   | `16`        | If the distance between SCP-575 and the player they are chasing becomes greater than this, SCP-575 uses the `movement_speed_fast` instead of `movement_speed`. |
| `scp575.kill_distance`                     | `0.8`       | If the distance between SCP-575 and the player they are chasing becomes smaller than this, the player is killed by SCP-575.                                    |
| `scp575.movement_speed_fast`               | `5.2`       | The movement speed SCP-575 uses if a significant gap has fallen to the player.                                                                                 |
| `scp575.movement_speed`                    | `4.2`       | The normal movement speed of SCP-575.                                                                                                                          |
| `scp575.light_points`                      | `30`        | Every 0.5 seconds, for each player shining a flashlight on SCP-575 a point is added. If it exceeds this treshold, SCP-575 will disappear.                      |
| `scp575.use_light_points`                  | `true`      | Whether SCP-575 can be defeated by shining flashlights on it.                                                                                                  |
| `command_responses`                        | ...         | Responses used for commands.                                                                                                                                   |

# Gameplay videos

Don't expect high quality, I compressed the videos.

**SCP-575 disappears due to being in a lighted room**

https://user-images.githubusercontent.com/36207738/213876283-ebdc666a-b313-421a-be9a-a52a524b667c.mp4

**SCP-575 chasing and killing his victim**

https://user-images.githubusercontent.com/36207738/213876270-b5333790-a3ed-462a-9e48-809bb9b982d5.mp4

**SCP-575 disappears due to being pointed at with a flashlight for too long**

https://user-images.githubusercontent.com/36207738/213876508-e25d35c8-0a54-4613-8634-2ecb53d6b7e2.mp4

# Credits

If you use any part of the code to create your own version of this plugin or pets, please remember to credit the original author.

# Unity Script Documentation (EN)

This project now uses a single split-component architecture. The former all-in-one `GameManager.cs` architecture was removed. The central script is now `GameSession.cs`, but it is intentionally a coordinator only: it owns game state and wires components together, while the actual gameplay work is handled by focused scripts.

## Architecture Summary

| Script | Responsibility |
| --- | --- |
| `GameSession.cs` | Coordinates game state, component references, start/game-over flow, and scene reload. |
| `StartMenuUI.cs` | Handles menu input, player name, character selection, and calls `GameSession.StartGame()`. |
| `HudUI.cs` | Displays score, player name, and the game-over panel. |
| `PlayerManager.cs` | Handles jump input, physics, grounded state, and obstacle collision. |
| `PlayerAppearance.cs` | Handles character sprite/controller selection and jump animation parameters. |
| `ObstacleSpawner.cs` | Spawns, moves, recycles, tags, and assigns point data to rocks. |
| `ObstacleData.cs` | Stores per-rock score value and optional clear sound. |
| `ScoreTracker.cs` | Detects valid rock jumps, awards points, applies milestone bonuses, and updates HUD. |
| `GroundScroller.cs` | Creates and scrolls floor tiles. |
| `BackgroundScroller.cs` | Scrolls the background material texture offset. |
| `AudioEffects.cs` | Loads and plays sound effects and ambience. |

## High-Level Flow

1. `GameSession.Start()` runs when the scene starts.
2. `GameSession` finds the player if it was not assigned.
3. `GameSession` ensures the focused components exist.
4. `GameSession` passes shared references to those components.
5. UI, HUD, ground tiles, obstacles, and score are initialized.
6. The game starts paused with menu visible and movement disabled.
7. `StartMenuUI` listens for start input.
8. `GameSession.StartGame()` begins the run.
9. Ground, background, obstacles, and scoring become active.
10. `PlayerManager` handles jump input.
11. `ScoreTracker` detects successful jumps over rocks.
12. `PlayerManager` ends the game when the player collides with an obstacle.
13. `GameSession` stops movement/scoring, shows game over, stops ambience, and lets `X` reload the scene.

## `GameSession.cs`

`GameSession` replaces the removed monolithic manager. It should stay small and mostly coordinate other scripts.

### Main State

- `inicio`: True after the game starts.
- `finJuego`: True after the player collides with an obstacle.
- `nombreJugador`: Player name displayed in the HUD.
- `indicePersonaje`: Selected character index. `0` is knight, `1` is snake.
- `gameOverHandled`: Private guard so game-over shutdown runs once.

### Scene References

These fields are assigned from the scene or migrated from the old manager component:

- `menuPrincipal`: Start menu object.
- `menugameOver`: Game-over menu object.
- `velocidad`: Shared movement speed passed to scrollers and obstacles.
- `col`: Ground tile prefab.
- `piedra1`: Large rock prefab.
- `piedra2`: Small rock prefab.
- `fondo`: Background renderer.
- `jugador`: Player transform.
- `campoNombre`: Name input field.
- `fuenteUI`: UI font.
- `previewPersonaje`: Character preview image.
- `textoPersonaje`: Character selector text.
- `minGapRocas` and `maxGapRocas`: Rock spacing range.

### Component References

`GameSession` owns references to all focused components:

- `StartMenuUI`
- `HudUI`
- `GroundScroller`
- `BackgroundScroller`
- `ObstacleSpawner`
- `ScoreTracker`
- `AudioEffects`
- `PlayerAppearance`
- `PlayerManager`

If a component reference is missing, `GameSession` adds or finds the component at runtime. Most session-level components are added to the same GameObject as `GameSession`. `PlayerAppearance` is added to the player object if missing.

### `Start()`

`Start()` initializes the architecture in this order:

1. `EnsurePlayerReference()` finds the player transform if `jugador` was not assigned.
2. `EnsureComponents()` gets or adds the focused components.
3. `ConfigureComponents()` passes shared references to those focused components.
4. `InitializeComponents()` runs setup methods such as `HudUI.Init()`, `StartMenuUI.Init()`, `GroundScroller.Init()`, and `ObstacleSpawner.Init()`.
5. `SetRunning(false)` keeps movement and scoring disabled until the player starts.

This order matters. Components need references before they initialize UI, spawn objects, or evaluate scoring.

### `Update()`

`GameSession.Update()` only handles the game-over phase. It does not move obstacles, update score, or read menu input.

When `inicio` and `finJuego` are both true:

1. `HandleGameOver()` stops active systems once.
2. Pressing `X` reloads the active scene.

### `StartGame()`

Starts one run if the game has not already started.

The method:

1. Reads player name from `StartMenuUI`, falling back to `campoNombre`.
2. Reads selected character index from `StartMenuUI`.
3. Applies the selected character through `PlayerAppearance`.
4. Sets `inicio = true`.
5. Sets `finJuego = false`.
6. Updates HUD player name.
7. Hides the game-over panel.
8. Resets score.
9. Hides the start menu.
10. Starts ambience.
11. Enables ground scrolling, background scrolling, obstacle movement, and score tracking.

### `EndGame()`

Called by `PlayerManager` when the player collides with an obstacle. It sets `finJuego = true` and plays the game-over sound.

### `SetRunning(bool value)`

This turns active gameplay systems on or off:

- `GroundScroller.SetRunning(value)`
- `BackgroundScroller.SetRunning(value)`
- `ObstacleSpawner.SetRunning(value)`
- `ScoreTracker.SetScoringEnabled(value)`

### `HandleGameOver()`

Runs once after the game ends:

- Stops all running gameplay systems.
- Shows the game-over UI.
- Stops ambience.

## `StartMenuUI.cs`

`StartMenuUI` owns the start menu behavior.

### Fields

- `menuPrincipal`: Menu root object.
- `campoNombre`: Text input for player name.
- `fuenteUI`: Font used for generated menu text.
- `previewPersonaje`: Image showing selected character.
- `textoPersonaje`: Text showing selected character state.
- `indicePersonaje`: Current character index.
- `playerAppearance`: Used to get preview sprites.
- `gameSession`: Session started by the menu.
- `menuActive`: Private flag that controls whether menu input is accepted.

### `Init()`

Finds/configures the name field, creates missing character selector UI, and updates the preview.

### `Update()`

If the menu is active:

- Left arrow selects the previous character.
- Right arrow selects the next character.
- `Enter` or keypad `Enter` starts the game.
- `X` starts the game only when the name input is not focused.

The input focus check prevents `X` and arrow controls from interfering while the player is typing a name.

### Character Selection

Character selection uses wraparound:

```csharp
indicePersonaje = (indicePersonaje + direccion + 2) % 2;
```

This keeps the selected index between `0` and `1`.

## `HudUI.cs`

`HudUI` manages runtime UI display.

### `Init()`

The HUD:

1. Copies a font from the menu if no font was assigned.
2. Finds the first `Canvas`.
3. Creates the score text if missing.
4. Creates the player-name text if missing.
5. Hides the game-over panel.
6. Refreshes the displayed player name.

### Methods

- `SetScore(int puntaje)`: Displays `Puntaje: <value>`.
- `SetPlayerName(string nombre)`: Stores and displays the player name.
- `ShowGameOver(bool show)`: Shows or hides the game-over menu.
- `ActualizarHUD()`: Refreshes HUD text currently stored in the component.

## `PlayerManager.cs`

`PlayerManager` handles player input, physics, and collision state. It no longer depends on the removed `GameManager`; it now depends on `GameSession`.

### Fields

- `fuerzaSalto`: Upward force applied when jumping.
- `EnSuelo`: Public read-only property for score logic.
- `controladorCaballero`: Knight animator controller.
- `controladorSerpiente`: Snake animator controller.
- `spriteCaballero`: Knight sprite.
- `spriteSerpiente`: Snake sprite.
- `gameSession`: Current game session.
- `rigidbody2D`: Cached player rigidbody.
- `animator`: Cached animator.
- `spriteRenderer`: Cached sprite renderer.
- `enSuelo`: Internal grounded state.

### `Update()`

Jumping only works when:

- `gameSession` exists.
- The game has started.
- The game is not over.
- The player is grounded.
- Space is pressed.

When those conditions pass:

1. `enSuelo` becomes false.
2. Jump animation flags turn on.
3. Upward force is applied.
4. Jump sound plays through `GameSession`.

### `OnCollisionEnter2D()`

- `Suelo`: marks player grounded and clears jump animation.
- `Obstaculo`: calls `gameSession.EndGame()`.

## `PlayerAppearance.cs`

`PlayerAppearance` isolates character visuals from player movement.

### `Awake()`

Caches `Animator` and `SpriteRenderer`. If knight defaults are missing, it uses the current animator controller and sprite.

### `AplicarPersonaje(int indice)`

- `0`: applies knight controller/sprite.
- `1`: applies snake controller/sprite.

After switching, it resets jump animation state.

### `SetSaltando(bool valor)`

Sets both animator bool names:

- `estaSaltando`
- `isJumping`

This keeps the code compatible with either animation parameter name.

## `ObstacleSpawner.cs`

`ObstacleSpawner` owns obstacle creation, movement, recycling, and point data.

### Fields

- `velocidad`: Obstacle movement speed.
- `piedra1`: Large rock prefab.
- `piedra2`: Small rock prefab.
- `minGapRocas`: Minimum gap between rocks.
- `maxGapRocas`: Maximum gap between rocks.
- `obstaculos`: Runtime obstacle list.
- `Obstaculos`: Read-only view of that list.
- `running`: Private movement flag.
- `obstaculoPuntuado`: Private list matching each obstacle to whether it already awarded score.

### `Init()`

If obstacles already exist, it does nothing. Otherwise:

1. Picks a random initial gap.
2. Spawns the large rock at x `14`, y `-2`.
3. Adds or updates `ObstacleData` on the large rock.
4. Adds the large rock to the obstacle list.
5. Marks it unscored.
6. Spawns the small rock after the random gap.
7. Adds or updates `ObstacleData` on the small rock.
8. Adds the small rock to the obstacle list.
9. Marks it unscored.

### Point Values

Current default point values:

```csharp
AsegurarObstacleData(grande, 100);
AsegurarObstacleData(pequena, 50);
```

To test the reward function quickly, change the large rock value to:

```csharp
AsegurarObstacleData(grande, 950);
```

### `Update()`

If `running` is true:

1. Recycle any obstacle at x `-10` or lower.
2. Move every obstacle left by `Time.deltaTime * velocidad`.

### `ReposicionarObstaculo(int index)`

Finds the rightmost other obstacle, places this obstacle after it by a random gap, and resets its scored flag.

### `AsegurarObstacleData(GameObject obstaculo, int puntos)`

Ensures the obstacle:

- Has tag `Obstaculo`.
- Has an `ObstacleData` component.
- Has the requested `puntos` value.

## `ObstacleData.cs`

Small data component used by obstacles.

- `puntos`: Points awarded for clearing this obstacle.
- `clearSfx`: Optional custom clear sound.

`ScoreTracker` reads `puntos`; `AudioEffects` reads both fields.

## `ScoreTracker.cs`

`ScoreTracker` owns all score logic.

### Fields

- `jugador`: Player transform.
- `puntaje`: Current score.
- `audioEffects`: Sound playback dependency.
- `hudUI`: HUD dependency.
- `obstacleSpawner`: Obstacle source.
- `playerController`: `PlayerManager` reference used to check grounded state.
- `siguienteBonus`: Next milestone threshold, starting at `1000`.
- `scoringEnabled`: Whether score detection is active.

### `AddPuntos(int cantidad)`

Adds points and applies milestone bonuses:

```csharp
puntaje += cantidad;

while (puntaje >= siguienteBonus)
{
    puntaje += 50;
    siguienteBonus += 1000;
}
```

The `while` allows multiple bonuses if a large score addition crosses more than one threshold.

Example:

1. Score is `950`.
2. Player clears a `50` point rock.
3. Score becomes `1000`.
4. Bonus triggers.
5. Score becomes `1050`.
6. Next threshold becomes `2000`.

### `EvaluarSaltoObstaculo(int index)`

This checks one obstacle.

It ignores the obstacle if:

- It was already scored.
- It has not passed the player yet.

A valid jump requires:

```csharp
bool enSuelo = playerController == null || playerController.EnSuelo;
bool saltoValido = !enSuelo &&
    jugador.position.y >= obstaculo.transform.position.y + 0.8f;
```

That means the player must be airborne and high enough above the obstacle.

If the jump is invalid and the obstacle is already far behind the player, the obstacle is marked scored anyway so it cannot be evaluated forever.

If valid:

1. The obstacle is marked scored.
2. `ObstacleData.puntos` is read.
3. `AddPuntos(puntos)` updates score and applies bonuses.
4. `AudioEffects.PlayClear(data)` plays the clear sound.

## `GroundScroller.cs`

Creates and scrolls ground tiles.

### `Init()`

If `col` is assigned and no tiles exist, it creates 21 tiles from x `-10` to x `10` at y `-3`.

### `Update()`

If running, each tile moves left. When a tile reaches x `-10`, it wraps back to x `10`.

## `BackgroundScroller.cs`

Scrolls the background renderer by changing the material texture offset.

Current logic:

```csharp
fondo.material.mainTextureOffset =
    fondo.material.mainTextureOffset + new Vector2(0.02f, 0) * Time.deltaTime;
```

The `velocidad` field exists, but the current offset speed is hard-coded to `0.02f`.

## `AudioEffects.cs`

Loads and plays audio.

### Loaded Resource Paths

- `Audio/piedra_grande`
- `Audio/piedra_pequena`
- `Audio/bonus`
- `Audio/salto`
- `Audio/game_over`
- `Audio/ambiente`

### Playback Methods

- `PlayJump()`
- `PlayGameOver()`
- `PlayBonus()`
- `PlayClear(ObstacleData data)`
- `StartAmbience()`
- `StopAmbience()`

`PlayClear()` uses a custom obstacle sound if one is assigned. Otherwise, rocks worth `100` or more use the large-rock sound, and lower values use the small-rock sound.

## Tags And Required Setup

Required tags:

- `Suelo`: Used by `PlayerManager` to detect landing.
- `Obstaculo`: Assigned by `ObstacleSpawner` and used by `PlayerManager` to detect game over.

Useful object names:

- `Player`: Fallback lookup used by `GameSession`.
- `CampoNombre`: Fallback lookup used by `StartMenuUI`.

Inspector references are still preferred because they are explicit and less fragile than name-based lookup.

## Common Change Locations

| Change | Location |
| --- | --- |
| Large rock score | `ObstacleSpawner.Init()` |
| Small rock score | `ObstacleSpawner.Init()` |
| Bonus amount | `ScoreTracker.AddPuntos()` |
| Bonus interval | `ScoreTracker.siguienteBonus` and `siguienteBonus += 1000` |
| Jump force | `PlayerManager.fuerzaSalto` in the Inspector |
| Obstacle speed | `GameSession.velocidad` or `ObstacleSpawner.velocidad` |
| Ground speed | `GameSession.velocidad` or `GroundScroller.velocidad` |
| Background scroll rate | `BackgroundScroller.Update()` |
| Character sprites/controllers | `PlayerManager` Inspector fields copied into `PlayerAppearance` |


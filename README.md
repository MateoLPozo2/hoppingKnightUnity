Documentación Compacta del Proyecto (Unity)

## Arquitectura
El proyecto utiliza una arquitectura modular coordinada por `GameSession.cs`, que administra el estado del juego y conecta los componentes. La lógica específica reside en scripts independientes.

| Script | Función |
|---|---|
| `GameSession` | Coordina el juego, inicio, fin y referencias. |
| `StartMenuUI` | Menú inicial, nombre y selección de personaje. |
| `HudUI` | HUD, puntaje y pantalla de Game Over. |
| `PlayerManager` | Movimiento, salto, físicas y colisiones. |
| `PlayerAppearance` | Sprites, animaciones y personaje activo. |
| `ObstacleSpawner` | Generación y reciclaje de obstáculos. |
| `ObstacleData` | Datos de puntuación y sonido por obstáculo. |
| `ScoreTracker` | Puntuación, bonos y validación de saltos. |
| `GroundScroller` | Movimiento del suelo. |
| `BackgroundScroller` | Desplazamiento del fondo. |
| `AudioEffects` | Reproducción de efectos y ambiente. |

## Flujo
1. `GameSession.Start()` inicializa componentes.
2. El juego comienza pausado mostrando el menú.
3. `StartMenuUI` inicia la partida.
4. Se activan suelo, fondo, obstáculos y puntuación.
5. `PlayerManager` controla el salto.
6. `ScoreTracker` otorga puntos al superar obstáculos.
7. Al colisionar con un obstáculo, `EndGame()` detiene el juego y muestra Game Over.
8. La tecla **X** reinicia la escena.

## Scripts

### GameSession
- Mantiene `inicio`, `finJuego`, nombre y personaje.
- Inicializa y conecta todos los componentes.
- Activa/desactiva sistemas mediante `SetRunning()`.
- Gestiona el Game Over y reinicio.

### StartMenuUI
- Gestiona nombre del jugador.
- Permite cambiar personaje.
- Inicia la partida con Enter o X.

### HudUI
- Muestra puntaje, nombre y Game Over.

### PlayerManager
- Gestiona salto y físicas.
- Solo permite saltar si el juego está activo y el jugador está en el suelo.
- Detecta colisiones con suelo y obstáculos.

### PlayerAppearance
- Cambia sprite/controlador según personaje.
- Reinicia estado de animación tras cambiar.

### ObstacleSpawner
- Genera obstáculos.
- Los recicla al salir de pantalla.
- Asigna puntuación:
  - Grande: 100
  - Pequeño: 50

### ObstacleData
- Guarda puntos y sonido opcional.

### ScoreTracker
- Detecta saltos válidos.
- Suma puntos.
- Cada 1000 puntos concede un bono de +50.

### GroundScroller
- Genera 21 baldosas y las recicla continuamente.

### BackgroundScroller
- Desplaza la textura del fondo.

### AudioEffects
Carga y reproduce:
- salto
- bonus
- game over
- ambiente
- sonidos de obstáculos

## Tags
- `Suelo`
- `Obstaculo`

## Objetos recomendados
- `Player`
- `CampoNombre`

## Cambios frecuentes

| Modificación | Ubicación |
|---|---|
| Puntos obstáculos | `ObstacleSpawner.Init()` |
| Bonificación | `ScoreTracker.AddPuntos()` |
| Fuerza de salto | Inspector (`PlayerManager`) |
| Velocidad | `GameSession` / `ObstacleSpawner` |
| Fondo | `BackgroundScroller.Update()` |
| Sprites y animaciones | `PlayerAppearance` |

<iframe src="https://www.linkedin.com/embed/feed/update/urn:li:ugcPost:7487140320283971585?collapsed=1" height="550" width="504" frameborder="0" allowfullscreen="" title="Embedded post"></iframe>

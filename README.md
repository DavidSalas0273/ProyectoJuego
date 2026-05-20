# 🗡️ Dungeons & Sword

Un juego de aventuras medieval desarrollado en Unity con combate en tercera persona, exploración de mazmorras y enfrentamientos épicos contra jefes.

## 🎮 Características del Juego

### ✅ **SISTEMAS IMPLEMENTADOS**
- **Cámara Third-Person** - Perspectiva desde el hombro del personaje
- **Menú Principal** - Con video de fondo y música medieval
- **Intro Star Wars** - Texto dorado que se desplaza verticalmente
- **Sistema de Combate** - Ataques ligeros/pesados, roll, auto-targeting
- **IA de Enemigos** - Persecución inteligente con barras de vida elegantes
- **Zonas de Curación** - 3 por escena con efectos de partículas rojas
- **Sistema de Portales** - Teletransporte entre escenas
- **Música Ambiental** - Medieval en MainMenu, Medieval Magical en Game
- **IA del Jefe** - Con ataques de proyectiles y animaciones
- **Sistema UI** - Barras de vida/stamina, pantalla de muerte
- **Sistema de Respawn** - Muerte y reaparición del jugador

### 🎯 **FLUJO DEL JUEGO**
```
MainMenu → Intro → Game → Villa/Dungeons/Cementerio/Jefe
```

### 🗺️ **ESCENAS DISPONIBLES**
- **MainMenu** - Menú principal con video de fondo
- **Intro** - Secuencia estilo Star Wars
- **Game** - Hub principal del juego
- **Villa** - Pueblo con enemigos esqueletos
- **Dungeons** - Mazmorras oscuras
- **Cementerio** - Área de combate
- **JefeMedio** - Enfrentamiento contra el jefe final

## 🔧 **Configuración del Proyecto**

### **Requisitos**
- Unity 2022.3 LTS o superior
- Input System Package
- Universal Render Pipeline (URP)
- TextMeshPro

### **Instalación**
1. Clona el repositorio:
   ```bash
   git clone https://github.com/DavidSalas0273/ProyectoJuego.git
   ```

2. **IMPORTANTE - Archivos de Música:**
   Los archivos de música fueron excluidos del repositorio por su tamaño. Para que el juego funcione correctamente con música, agrega estos archivos a `Assets/Musica/`:
   - `Medieval Magical Music - 5 Min - Royalty Free.mp3`
   - `Medieval Music for Focus & Relaxation _ The Gray Wizard's Journey [LCfEqudu4pc].mp3`
   - `Medieval.mp3`

3. Abre el proyecto en Unity

4. Configura las escenas en Build Settings:
   - MainMenu (índice 0)
   - Intro (índice 1)
   - Game (índice 2)
   - Villa, Dungeons, Cementerio, JefeMedio

## 🎮 **Controles**

### **Movimiento**
- **WASD** - Movimiento del personaje
- **Shift Izquierdo** - Correr
- **Espacio** - Roll/Esquivar

### **Combate**
- **Click Izquierdo** - Ataque ligero
- **Click Derecho** - Ataque pesado
- **Auto-targeting** - El personaje rota automáticamente hacia el enemigo más cercano

### **Navegación**
- **Cualquier tecla** - Saltar intro
- **Botones UI** - Navegación por menús

## 🏗️ **Arquitectura del Código**

### **Scripts Principales**
- `PlayerController.cs` - Control del personaje y combate
- `Camara.cs` - Sistema de cámara third-person
- `EnemyAI.cs` - Inteligencia artificial de enemigos
- `EnemyHealth.cs` - Sistema de vida con barras elegantes
- `JefeIA.cs` - IA del jefe con ataques especiales
- `ZonaCuracion.cs` - Zonas de curación con partículas
- `MainMenuManager.cs` - Gestión del menú principal
- `IntroStarWars.cs` - Secuencia de introducción

### **Sistemas de Audio**
- `MusicaMenu.cs` - Música del menú principal
- `MusicaAmbiente.cs` - Música ambiental de las escenas

### **Gestión de Escenas**
- `Portal.cs` - Teletransporte entre escenas
- `GameManager.cs` - Gestión global del juego
- `PlayerPersistente.cs` - Persistencia de datos del jugador

## 🎨 **Assets Utilizados**

### **Modelos 3D**
- KayKit Skeletons (Enemigos esqueletos)
- Kenney Graveyard Kit (Cementerio)
- Kenney Mini Dungeon (Mazmorras)
- Modular Village Collection (Villa)

### **Audio**
- Música medieval para ambientación
- Efectos de sonido de combate

### **Efectos Visuales**
- Sistema de partículas para zonas de curación
- Efectos de combate y proyectiles

## 🚀 **Mejoras Técnicas Recientes**

- ✅ **Cámara mejorada** - Perspectiva desde el hombro sin control del usuario
- ✅ **Movimiento optimizado** - Relativo al mundo en lugar de la cámara
- ✅ **Configuración MCP** - Para integración con Unity
- ✅ **Sistema de música** - Reproducción automática por escena
- ✅ **Barras de vida elegantes** - Aparecen sobre la cabeza de los enemigos

## 📝 **Notas de Desarrollo**

Este proyecto fue desarrollado como parte de un ejercicio de game development, implementando sistemas completos de:
- Combate en tercera persona
- IA de enemigos y jefes
- Gestión de escenas y UI
- Sistemas de audio y efectos visuales
- Arquitectura modular y escalable

## 🤝 **Contribuciones**

Las contribuciones son bienvenidas. Por favor:
1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 **Licencia**

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles.

---

**Desarrollado con ❤️ usando Unity**
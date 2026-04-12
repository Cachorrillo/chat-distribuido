# Chat distribuido

Proyecto de investigación orientado al desarrollo de un sistema de chat distribuido utilizando sockets TCP, concurrencia y mecanismos básicos de alta disponibilidad.

## Descripción general

El proyecto consiste en diseñar e implementar un sistema de comunicación cliente-servidor capaz de atender múltiples clientes de forma concurrente. Además de la funcionalidad básica de chat, el enfoque principal está en la calidad del sistema, especialmente en aspectos como disponibilidad, tolerancia a fallos y continuidad del servicio.

Como estrategia principal, se plantea una arquitectura con servidor principal y servidor de respaldo, junto con mecanismos de reconexión del cliente, con el fin de demostrar alta disponibilidad básica.

---

## Tecnologías utilizadas

- C#
- .NET
- Sockets TCP
- Docker
- WSL / Linux
- Git / GitHub

---

## Estructura del repositorio

- `docs/semana11.md`: definición del tema, objetivos y alcance
- `docs/semana12.md`: metodología, herramientas, recursos y plan de trabajo
- `docs/semana13.md`: diseño del sistema
- `docs/semana14.md`: alta disponibilidad y tolerancia a fallos
- `src/Server`: código del servidor
- `src/Client`: código del cliente
- `docker/`: archivos de despliegue con contenedores

---

## Requisitos

Para ejecutar el proyecto necesitas:

- Docker instalado
- Docker Compose
- (Opcional) WSL en Windows

---

## 🚀 Cómo ejecutar el proyecto

### 1. Clonar el repositorio

```bash
git clone https://github.com/TU_USUARIO/chat-distribuido.git
cd chat-distribuido

docker compose up --build -d

docker run -it --rm --network chat-distribuido_default chat-distribuido-client1 --> esto para abrir nuevas terminales y emulen clientes

docker stop server-main ---> detener el servidor principal para autoconectarse al backup

docker start server-main ----> encender el servidor principal y se conecte denuevo a el de manera automatica
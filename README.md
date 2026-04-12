# Chat distribuido

Proyecto de investigación orientado al desarrollo de un sistema de chat distribuido utilizando sockets TCP, concurrencia y mecanismos básicos de alta disponibilidad.

## Descripción general

El proyecto consiste en diseñar e implementar un sistema de comunicación cliente-servidor capaz de atender múltiples clientes de forma concurrente. Además de la funcionalidad básica de chat, el enfoque principal estará en la calidad del sistema, especialmente en aspectos como disponibilidad, tolerancia a fallos y continuidad del servicio.

Como estrategia principal, se plantea una arquitectura con servidor principal y servidor de respaldo, junto con mecanismos de reconexión del cliente, con el fin de demostrar alta disponibilidad básica.

## Tecnologías previstas

- C#
- .NET
- Sockets TCP
- Docker
- WSL / Linux
- GitHub

## Estructura del repositorio

- `docs/semana11.md`: definición del tema, objetivos y alcance
- `docs/semana12.md`: metodología, herramientas, recursos y plan de trabajo
- `docs/semana13.md`: diseño del sistema
- `docs/semana14.md`: Alta disponibilidad y tolerancia a fallos
- `src/Server`: código del servidor
- `src/Client`: código del cliente
- `docker/`: archivos de despliegue con contenedores

## Estado del proyecto

Actualmente se encuentra en fase de planificación y definición del alcance.

## Licencia

Este proyecto se distribuye bajo la licencia MIT.

## Autor

Víctor Manuel Solera Hernández
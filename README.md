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
- `docs/semana15.md`: documento final
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

## Ejecución del proyecto (paso a paso)

A continuación se describe el proceso completo para ejecutar el sistema de chat distribuido utilizando Docker.

---

### 1. Clonar el repositorio

```bash
git clone "direccion"
cd chat-distribuido
```

---

### 2. Levantar los servidores

```bash
docker compose up --build -d
```

Esto iniciará los siguientes servicios:

* `server-main` → servidor principal (puerto 5000)
* `server-backup` → servidor de respaldo (puerto 5001)

---

### 3. Construir la imagen del cliente

```bash
docker compose build client-template
```

Este paso genera la imagen `chat-distribuido-client`, la cual será utilizada para crear instancias de clientes interactivos.

---

###  4. Ejecutar clientes (modo interactivo)

Para simular usuarios, abre una nueva terminal por cada cliente y ejecuta:

```bash
docker run -it --rm --network chat-distribuido_default chat-distribuido-client
```

Puedes abrir múltiples clientes repitiendo el mismo comando en diferentes terminales.

---

### 5. Probar el chat

* Escribe mensajes en cualquier cliente
* Verifica que los demás clientes los reciben en tiempo real

---

##  Prueba de alta disponibilidad

###  Simular fallo del servidor principal

```bash
docker stop server-main
```

Comportamiento esperado:

* Los clientes detectan la caída
* Se reconectan automáticamente al servidor de respaldo (`BACKUP`)
* El chat continúa funcionando

---

### Restaurar el servidor principal

```bash
docker start server-main
```

Comportamiento esperado:

* Los clientes detectan que el servidor principal volvió
* Se reconectan automáticamente a `MAIN`
* El chat continúa sin interrupciones

---

## Detener el sistema

```bash
docker compose down
```

---

## Nota importante

El nombre de la red Docker puede variar dependiendo del nombre de la carpeta del proyecto.
Para verificarlo, puedes usar:

```bash
docker network ls
```

En caso de ser diferente, reemplaza `chat-distribuido_default` en el comando del cliente por el nombre correcto.

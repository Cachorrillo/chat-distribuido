\# Semana 13 - Revisión Bibliográfica y Marco Teórico



\##  Introducción



En esta sección se presenta la revisión bibliográfica y el marco teórico que fundamenta el desarrollo del proyecto de chat distribuido con tolerancia a fallos. Se abordan los conceptos clave necesarios para comprender el funcionamiento del sistema, incluyendo sockets, concurrencia, sistemas distribuidos, alta disponibilidad y tolerancia a fallos.



\---



\##  1. Sockets y comunicación TCP



Los sockets son un mecanismo fundamental para la comunicación entre procesos en una red. En el contexto de este proyecto, se utilizan sockets TCP para establecer conexiones confiables entre el servidor y múltiples clientes.



El protocolo TCP (Transmission Control Protocol) garantiza:



\- Entrega ordenada de los datos

\- Control de errores

\- Reintentos automáticos de transmisión

\- Conexión orientada a flujo



Estas características lo hacen ideal para aplicaciones como sistemas de chat, donde la integridad de los mensajes es fundamental.



En .NET, los sockets se implementan mediante clases como `TcpListener` y `TcpClient`, que permiten aceptar conexiones y enviar/recibir datos a través de streams.



\---



\##  2. Concurrencia en sistemas de red



La concurrencia es la capacidad de un sistema para manejar múltiples tareas simultáneamente. En aplicaciones de red, es esencial para atender múltiples clientes sin bloquear el servidor.



En este proyecto, la concurrencia se logra mediante:



\- Tareas asíncronas (`Task`)

\- Procesamiento independiente por cliente

\- Uso de estructuras seguras para múltiples hilos (como `ConcurrentDictionary`)



Esto permite que el servidor:



\- Acepte múltiples conexiones simultáneas

\- Procese mensajes de varios clientes en paralelo

\- Evite bloqueos que afecten a todo el sistema



\---



\##  3. Sistemas distribuidos



Un sistema distribuido está compuesto por múltiples nodos que cooperan para ofrecer un servicio. Estos nodos pueden estar en diferentes máquinas o procesos, pero funcionan como un sistema unificado.



Las características principales de los sistemas distribuidos incluyen:



\- Comunicación entre nodos mediante red

\- Independencia de fallos

\- Escalabilidad

\- Transparencia para el usuario



En este proyecto, el sistema distribuido se representa mediante:



\- Un servidor principal (MAIN)

\- Un servidor de respaldo (BACKUP)

\- Múltiples clientes conectados



\---



\##  4. Tolerancia a fallos



La tolerancia a fallos es la capacidad de un sistema para continuar operando incluso cuando uno de sus componentes falla.



En el contexto de este proyecto, se implementa mediante:



\- Redundancia de servidores (MAIN y BACKUP)

\- Detección de desconexión

\- Reconexión automática del cliente



Esto permite que, ante la caída del servidor principal, el sistema continúe funcionando mediante el servidor de respaldo.



\---



\##  5. Alta disponibilidad



La alta disponibilidad se refiere a la capacidad de un sistema para mantenerse operativo la mayor parte del tiempo, minimizando el tiempo de inactividad.



Se logra mediante:



\- Redundancia de componentes

\- Mecanismos de recuperación automática

\- Diseño orientado a fallos



En este proyecto, la alta disponibilidad se implementa utilizando un modelo \*\*activo-respaldo\*\*, donde:



\- El servidor MAIN atiende las solicitudes

\- El servidor BACKUP entra en funcionamiento cuando el principal falla



\---



\##  6. Escalabilidad



La escalabilidad es la capacidad de un sistema para manejar un aumento en la carga de trabajo.



Existen dos tipos principales:



\- Escalabilidad vertical: aumentar recursos en una misma máquina

\- Escalabilidad horizontal: agregar más nodos al sistema



Este proyecto permite escalabilidad horizontal básica mediante:



\- Soporte para múltiples clientes concurrentes

\- Posibilidad de agregar más servidores en el futuro



\---



\##  7. Seguridad básica en sistemas de red



Aunque este proyecto no está enfocado en seguridad avanzada, se consideran aspectos básicos como:



\- Validación de mensajes

\- Manejo de excepciones para evitar caídas del sistema

\- Uso de TCP para garantizar integridad de datos



En futuras mejoras, se podrían incluir:



\- Autenticación de usuarios

\- Encriptación de mensajes

\- Control de acceso



\---



\##  8. Métricas y evaluación del sistema



Para evaluar el desempeño del sistema, se pueden considerar métricas como:



\- Tiempo de respuesta

\- Tiempo de reconexión ante fallos

\- Tiempo de inactividad (downtime)

\- Número de clientes concurrentes soportados



Estas métricas permiten analizar la calidad del sistema y proponer mejoras.



\---



\##  Referencias bibliográficas



\- Tanenbaum, A. S., \& Van Steen, M. (2017). \*Distributed Systems: Principles and Paradigms\*. Pearson.

\- Coulouris, G., Dollimore, J., Kindberg, T., \& Blair, G. (2011). \*Distributed Systems: Concepts and Design\*. Addison-Wesley.

\- Sommerville, I. (2016). \*Software Engineering\*. Pearson.

\- Microsoft Docs. (2024). \*TCP Classes (System.Net.Sockets)\*. Recuperado de: https://learn.microsoft.com

\- Kurose, J. F., \& Ross, K. W. (2017). \*Computer Networking: A Top-Down Approach\*. Pearson.



\---



\##  Conclusión



El marco teórico presentado proporciona la base conceptual necesaria para el desarrollo del sistema de chat distribuido. Los conceptos de sockets, concurrencia y sistemas distribuidos son fundamentales para la implementación, mientras que la alta disponibilidad y la tolerancia a fallos permiten garantizar la continuidad del servicio.



Estos fundamentos teóricos respaldan las decisiones de diseño adoptadas en el proyecto y permiten evaluar su comportamiento en escenarios reales de fallo.


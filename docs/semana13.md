\# Semana 13 – Diseño del sistema



\## Arquitectura del sistema



El sistema propuesto seguirá una arquitectura cliente-servidor distribuida, compuesta por múltiples clientes, un servidor principal y un servidor de respaldo.



Los clientes se conectarán inicialmente al servidor principal, el cual será responsable de recibir, procesar y distribuir los mensajes entre los usuarios conectados. Para garantizar la disponibilidad del sistema, se incorporará un servidor de respaldo que permitirá mantener la continuidad del servicio en caso de fallo del servidor principal.



Esta arquitectura permite implementar mecanismos básicos de alta disponibilidad sin requerir configuraciones complejas de replicación o consenso.



\## Componentes del sistema



\### Cliente

Aplicación encargada de conectarse al servidor, enviar mensajes y recibir mensajes en tiempo real.



\### Servidor principal (MAIN)

Responsable de:

\- Aceptar conexiones de múltiples clientes

\- Gestionar la comunicación

\- Distribuir mensajes a todos los clientes conectados



\### Servidor de respaldo (BACKUP)

Encargado de:

\- Estar disponible en caso de fallo del servidor principal

\- Aceptar conexiones cuando el servidor principal no esté disponible



\## Flujo de comunicación



1\. El cliente intenta conectarse al servidor principal.

2\. Si la conexión es exitosa, comienza la comunicación normal.

3\. El cliente envía mensajes al servidor.

4\. El servidor distribuye los mensajes a todos los clientes conectados.

5\. Si el servidor principal falla, el cliente detecta la desconexión.

6\. El cliente intenta conectarse automáticamente al servidor de respaldo.

7\. La comunicación continúa a través del servidor de respaldo.



\## Manejo de fallos



El sistema implementará un mecanismo básico de tolerancia a fallos mediante:



\- Detección de desconexión del servidor principal.

\- Intentos de reconexión automática.

\- Cambio de conexión hacia un servidor alterno.



Este enfoque permite mantener la disponibilidad del sistema ante fallos simples, alineándose con los objetivos del proyecto.



\## Consideraciones de diseño



\- Se utilizarán sockets TCP para garantizar la confiabilidad de la comunicación.

\- Se implementará concurrencia en el servidor para soportar múltiples clientes simultáneos.

\- El sistema estará preparado para ejecutarse en entornos Linux mediante contenedores Docker.


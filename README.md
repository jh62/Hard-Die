# Hard To Kill

### Entrega del TP Final 2.

Resumen: Intenté recrear las mecánicas básicas del juego Die Hard Trilogy de PSX, aplicando los conocimientos que aprendimos durante el curso.

La build cuenta con 1 nivel y enemigos que patrullan los alrededores. El objetivo es eliminarlos a todos.

Los controles son simples: se mueve el personaje con WASD y se dispara con el botón izquierdo del mouse. Adiocionalmente, el mouse mueve la vista horizontalmente.

El sistema de apuntar no es el convencional, sino que el personaje dispara a todo aquello que esté dentro de un box collider, por lo que apunta automáticamente a los enemigos que estén por debajo o por encima de su altura, siempre priorizando a los objetivos más cercanos.

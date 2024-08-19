# AdventChatBack
## Configuración
1. Copia `appsettings.json.template` a `appsettings.json`.
2. Reemplaza los placeholders en `appsettings.json` con tus propias claves y configuraciones.

   aplicación de Generación Aumentada por Recuperación (RAG) completamente en C# .NET. Esta solución web de 4 capas demuestra cómo integrar tecnologías de IA avanzadas en el ecosistema .NET, ofreciendo una perspectiva única sobre el procesamiento de lenguaje natural y la recuperación de información.
Arquitectura del proyecto:
API: Punto de entrada con dos endpoints principales: Procesamiento de la base de conocimiento Manejo de prompts del usuario
Application: Lógica de negocio y orquestación de servicios
Domain: Definición de entidades y lógica de dominio
Infrastructure: Implementación de servicios externos e internos

![image](https://github.com/user-attachments/assets/d0853f4a-16cd-437c-8379-91680a9bc3d9)

Solución .Net 7 / VS 2022

![image](https://github.com/user-attachments/assets/a88cd818-7f9b-457a-b9b3-4683c6ef7031)

Servicios Responsables de Operación del RAG

![image](https://github.com/user-attachments/assets/0d448e67-979d-4028-b1b4-1c697aabe4fe)

Diagrama de Operación de un RAG
Explicación del Funcionamiento de una Aplicación RAG (Retrieval-Augmented Generation):

Usuario: Todo comienza cuando el usuario realiza una consulta a la aplicación RAG. Este puede ser cualquier tipo de pregunta o solicitud de información.
Aplicación RAG: La consulta ingresada es procesada por la aplicación RAG, que se encarga de gestionar y coordinar los componentes del sistema para generar una respuesta adecuada.
Retriever (Componente de Recuperación): La aplicación envía la consulta al Retriever, un componente clave que busca documentos relevantes en la Base de Conocimientos. Esta base puede estar compuesta por información interna o actualizada desde Fuentes Externas. El Retriever se asegura de obtener los datos más pertinentes que se usarán para construir la respuesta.
Generador LLM (Large Language Model): Una vez que el Retriever ha recuperado los documentos relevantes, estos se envían al Generador LLM, que utiliza esta información como contexto para generar una respuesta completa y contextualizada para el usuario.
Respuesta Final: Finalmente, la respuesta generada es presentada al usuario, completando el ciclo del sistema.

Componentes clave y su funcionamiento:
1. Gestión de datos con Google Firebase:
Utilizamos Firebase para almacenar nuestra base de conocimiento en archivos .txt. El GoogleFirebaseService se encarga de recuperar estos archivos:

![image](https://github.com/user-attachments/assets/922d80b9-1b71-4a30-ad44-08a8747a00f4)

ProcessingDataService


2. Procesamiento y chunking semántico:
El ChunkingService divide los textos en fragmentos semánticos, preparándolos para su conversión a embeddings:

![image](https://github.com/user-attachments/assets/6314d820-0879-4ee0-9c37-2cdfbf89f371)

ChunkingService


3. Generación de embeddings con Cohere:
Utilizamos la API de Cohere para generar embeddings de alta calidad:

![image](https://github.com/user-attachments/assets/5de9756e-c3ef-44c8-abc0-7589250935f8)

EmbeddingService


4. Almacenamiento vectorial con Pinecone:
Los embeddings se almacenan en Pinecone para búsquedas semánticas eficientes:

![image](https://github.com/user-attachments/assets/45bfa037-b5ab-4722-94b3-8f07e3bf34bd)

EmbeddingStoregeServices


5. Generación de respuestas con OpenAI:
Utilizamos la API de OpenAI para generar respuestas contextuales:

![image](https://github.com/user-attachments/assets/0664eecc-d32d-46b7-8a81-8decf7c133d4)

GenerativeResponseService

Tecnologías clave:
C# .NET
Firebase Storage
Cohere (Embeddings)
Pinecone (Base de datos vectorial)
OpenAI GPT (Generación de respuestas)
Este proyecto demuestra cómo las tecnologías .NET pueden integrarse perfectamente con servicios de IA de vanguardia para crear aplicaciones RAG potentes y escalables.
¿Trabajas con C# .NET o estás interesado en aplicaciones RAG? ¡Me encantaría conocer tu opinión o responder a tus preguntas! Comenta abajo o contáctame para más detalles.
#CSharp #DotNet #RAG #AI #MachineLearning #NLP #VectorDatabases

Fuente:https://www.linkedin.com/pulse/desarroll%C3%A9-una-aplicaci%C3%B3n-rag-completa-en-c-net-un-viaje-arlex-guzman-ffrye/?trackingId=TEqbVgRdSAqrGVz%2BzFKTFA%3D%3D 

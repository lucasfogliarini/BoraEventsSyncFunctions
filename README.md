## O que é isso, GPT?

1. **MinhaEntradaSync**: Este código é uma função do [Azure Functions](https://azure.microsoft.com/pt-br/products/functions) escrita em C# que extrai eventos do site [Minha Entrada](https://minhaentrada.com.br/) usando o `MinhaEntradaCrawler`. A função é acionada por um Timer Trigger e roda de acordo com a expressão cron configurada. Ela pega os eventos da próxima semana, formata-os em um formato mais simples e os envia para uma fila do [Azure Service Bus](https://azure.microsoft.com/pt-br/products/service-bus/).

ChatGPT Version:
https://help.openai.com/en/articles/6825453-chatgpt-release-notes

# Bora Sincronizador de Eventos

Este repositório contém uma coleção de funções do [Azure Functions](https://azure.microsoft.com/pt-br/products/functions) desenvolvidas em C# que têm como objetivo extrair informações sobre eventos de diferentes sites na internet e armazená-las em uma fila do [Azure Service Bus](https://azure.microsoft.com/pt-br/products/service-bus/). Cada função é acionada por um Timer Trigger e é configurada para rodar de acordo com uma expressão cron específica.  
A função `GoogleCalendarSync` é acionada automaticamente sempre que uma mensagem é adicionada à fila extraindo informações do evento criado a partir da mensagem, como título, data de início e link do evento.

## TeatroMinhaEntradaSync

A função **TeatroMinhaEntradaSync** tem como objetivo extrair eventos de [Teatro em Porto Alegre](https://minhaentrada.com.br/agenda-geral?estado=RS&cidade=7994&categoria=4) do site [Minha Entrada](https://minhaentrada.com.br) utilizando o `MinhaEntradaCrawler`. Esta função é especialmente útil para quem deseja acompanhar os eventos teatrais na região de Porto Alegre. Os eventos da próxima semana são coletados, formatados em um formato mais simples e enviados para uma fila.

## StandUpSymplaSync

A função **StandUpSymplaSync** é responsável por extrair informações sobre eventos de [StandUp em Porto Alegre](https://www.sympla.com.br/eventos/porto-alegre-rs/stand-up-comedy) do site [Sympla](https://www.sympla.com.br) usando o `SymplaCrawler`. Isso facilita a busca por eventos de comédia stand-up na região. Os eventos previstos para a próxima semana são extraídos, formatados e enviados para uma fila.

## ValenEventbriteSync

A função **ValenEventbriteSync** concentra-se na extração de eventos da [Valen](https://www.eventbrite.com/o/valen-bar-24177627927) do site [Eventbrite](https://www.eventbrite.com) com o auxílio do `EventbriteCrawler`. Ela permite que os interessados em eventos da Valen fiquem atualizados sobre o que está acontecendo. Assim como as outras funções, os eventos da próxima semana são coletados, formatados e enviados para uma fila.

## GoogleCalendarSync

A função **GoogleCalendarSync** é responsável por sincronizar eventos recebidos de uma fila do Azure Service Bus com o serviço Google Calendar.

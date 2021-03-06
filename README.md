[![Twitter](https://raw.githubusercontent.com/Chaotica-IOChat/BotPostRouting/master/BotPostRouting/twitter-bird.png)](https://twitter.com/ChaoticaDev)

# BotPostRouting
Simple routing for Microsoft Bot Framework, Conversation routing.

BotPostRouting implements a routing-like REST API equivalent to `$_GET['value']` in php.

We strongly recommend reading documentation on the [wiki-page](https://github.com/Chaotica-IOChat/BotPostRouting/wiki)! :)

---

## FauxConversationController

`FauxConversationController` is a controller which handles routing and contains all conversation points + routes.

## PointInConversation

`PointInConversation` is a class which keeps track of the current point in the conversation

## ConversationRoute

`ConversationRoute` is an HTTP_POST router. ie: "/home"


---

## Routing

![Screenshot](https://raw.githubusercontent.com/Chaotica-IOChat/BotPostRouting/master/BotPostRouting/bot-route.gif)

    public async void Route(String _GET, ConversationRouteType routeType, bool wildcard_allowed = true, Func<Task<bool>> func = null)

    route.Route("/home", ConversationRouteType.EXACT_MATCH, true, async () =>
    {
        //Routed = true
        isRouted = true;

        //Set point in conversation = route PointInConversation
        currentConvoPoint = route.PIC;

        //Post route response
        await context.PostAsync($"{userName}, {route.PIC.ResponseText}");

        return true;
    });
    
    

# BotPostRouting
Simple routing for Microsoft Bot Framework, Conversation routing.

BotPostRouting implements a routing-like REST API equivalent to `$_GET['value']` in php.

---

## FauxConversationController

`FauxConversationController` is a controller which handles routing and contains all conversation points + routes.

## PointInConversation

`PointInConversation` is a class which keeps track of the current point in the conversation

## ConversationRoute

`ConversationRoute` is a class which handles possible routes at `PointInConversation`.


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
    
    

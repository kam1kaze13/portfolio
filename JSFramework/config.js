import { errorHandler } from './errorHandler'

export const config = (container) =>
({
    routes : [],
    filters : [],
    hooks : [],
    currController : "base",
    container,

    defineController(controllerName)
    {
        this.currController = controllerName;
    },

    inject(depName)
    {
        let di = this.container.get(depName);
        
        if (di === undefined)
        {
            di = this.container.getSingleton(depName);
        }
        
        if (di === undefined)
        {
            return;
        }
        
        this[depName] = di;
    },

    addRoute(httpMethod, route, func)
    {
        this.routes.push({
            method: httpMethod, 
            pattern: new RegExp('^' + route.replace(/{\w+}/g,'(\\w+)') + '$'),
            callback: func});
    },

    addFilter(filter)
    {
        this.filters.push(filter);
    },

    register(hookName)
    {
        this.hooks.push({hookName : hookName, handler: () => {}});
    },

    hook(hookName, handler)
    {
        if (typeof hookName === 'string')
        {
            let hook = this.hooks.find(elem => elem.hookName == hookName);

            if (hook)
            {
                let index = this.hooks.indexOf(hook);

                this.hooks[index].handler = handler;
            }
            else
            {
                return errorHandler.createResponse500(1.1);
            } 
        }
        else if (typeof hookName === 'object')
        {
            for (var key in hookName)
            {
                let hook = this.hooks.find(elem => elem.hookName == key);

                if (hook)
                {
                    let index = this.hooks.indexOf(hook);

                    this.hooks[index].handler = hookName[key];
                }
            }
        }
        else
        {
            return errorHandler.createResponse500(1.1);
        }
    },

    raise(hookName, args)
    {
        let hook = this.hooks.find(elem => elem.hookName == hookName);

        if (hook)
        {
            let index = this.hooks.indexOf(hook);

            this.hooks[index].handle.apply(args);
        }
        else
        {
            return errorHandler.createResponse500(1.1);
        } 
    },

    handle(httpMethod, path, request, response)
    {        
        var i = this.routes.length;

        while(i--) {
            
            if (this.routes[i].method == httpMethod)
            {
                var args = path.match(this.routes[i].pattern);
           
                if(args) {
                   
                    this.routes[i].callback.apply(this.args.slice(1));
                }
            }         
        }
    }
});
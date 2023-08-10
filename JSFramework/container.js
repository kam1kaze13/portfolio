export const container = _ =>  
({
    singletonArray : [],
    functionArray : [],

    addSingleton(name, singleton)
    {
        this.singletonArray.push({elemName : name, elemSingleton : singleton});
    },

    add(name, func)
    {
        this.functionArray.push({elemName: name, elemFunc: func});
    },

    getSingleton(name)
    {
        const target = this.singletonArray.find(element => element.elemName === name);
        if (target === undefined)
        {
            return;
        }
            
        return target.elemSingleton;
    },

    get(name)
    {
        const target = this.functionArray.find(element => element.elemName === name);
        if (target === undefined)
        {
            return;
        }
            
        return target.elemFunc();
    }
});
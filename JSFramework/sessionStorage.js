export const sessionStorage =
{
    storage : [],

    store(sessionId, session)
    {
        this.storage.push({id: sessionId, sess: session});
    },

    getSession(sessionId)
    {
        const target = this.storage.find(elem => elem.id == sessionId);

        if(target == null)
        {
            return;
        }

        return target;
    },

    deleteSession(sessionId)
    {
        this.storage = this.storage.filter(function (obj) {
            return obj.sessionId !== sessionId;
        });
    }
}
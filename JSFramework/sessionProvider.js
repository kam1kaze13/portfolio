const sessionProvider = (memorySessionStorage) => 
({
    sessionStorage : memorySessionStorage,
    defaultExpirationTimeSpan : 10,

    login(id, roles, expirationTimeSpan)
    {
        this.sessionStorage.store(id, {roles : roles, time : expirationTimeSpan});
    },

    logout(sessionId)
    {
        this.sessionStorage.deleteSession(sessionId);
    },

    getSession(sessionId)
    {
        return this.sessionStorage.getSession(sessionId);
    }
})
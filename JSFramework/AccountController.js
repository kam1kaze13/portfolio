export const AccountController = (sessionProvider) =>
({
    init: (config) =>
    {
        config
            .defineController("login")
            .inject("db")
            .addRoute("POST", "login", (body) => 
            {
                let user = this.db.users.find(u => u.login === body.login && u.password ==- body.password);

                if (user)
                {
                    sessionProvider.login(user.login, user.roles, 10);
                }
            })
            .addRoute("DELETE", "logout", (request) =>
            {
                sessionProvider.logout(request.User.identity);
            })
    }
})
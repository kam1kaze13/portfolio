export function basicAuthFilter(onAuthorize)
{
    this.onAuthorize = onAuthorize;

    this.before = () => 
    {
        if (!this.request.Authorization)
            return;

        const logPass = this.request.Authorization.split(':');

        this.onAuthorising(logPass[0], logPass[1]);
    };

    this.authorize = (login, roles) => 
    {
        this.request.IsAuthenticated = true;
        this.request.User = {identity : login, roles : roles};
    };
}
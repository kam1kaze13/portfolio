export const TaskController = 
{
    init: (config) =>
    {
        config
            .defineController("task")
            .inject("db")
            .addRoute("GET", "/task/{taskId}", (taskId) =>
            {
                let task = this.db.tasks.find(elem => elem.id === taskId);

                if (task)
                {
                    return task.description;
                }

                return null;
            })
            .addRoute("GET", "/tasks", () =>
            {
                return this.db.tasks;
            })
            .addRoute("POST", "/tasks", (body) =>
            {
                this.db.tasks.push({id: this.db.tasks.length + 1, description : body});
            })
            .addRoute("DELETE", "/task/{taskId}", (taskId) =>
            {
                let task = this.db.tasks.find(elem => elem.id === taskId);

                if (task)
                {
                    let index = this.db.tasks.indexOf(task);

                    this.db.tasks.splice(index, 1);
                }               
            })
            .addRoute("PUT", "/task/{taskId}", (taskId, body) =>
            {
                let task = this.db.tasks.find(elem => elem.id === taskId);
                if (task)
                {
                    let index = this.db.tasks.indexOf(task);

                    this.db.tasks[index].description = body;
                }
            })
    }
};
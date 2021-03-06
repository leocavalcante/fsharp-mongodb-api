namespace Todos.Http

open Giraffe
open Microsoft.AspNetCore.Http
open Todos
open FSharp.Control.Tasks.V2
open System

module TodoHttp =
    let handlers : HttpFunc -> HttpContext -> HttpFuncResult =
        choose
            [ POST >=> route "/todos" >=> fun next context ->
                  task {
                      let save = context.GetService<TodoSave>()
                      let! todo = context.BindJsonAsync<Todo>()
                      let todo =
                          { todo with Id = ShortGuid.fromGuid (Guid.NewGuid()) }
                      return! json (save todo) next context
                  }
              GET >=> route "/todos" >=> fun next context ->
                  let find = context.GetService<TodoFind>()
                  let todos = find TodoCriteria.All
                  json todos next context

              PUT
              >=> routef "/todos/%s"
                      (fun id next context -> text ("Update " + id) next context)

              DELETE
              >=> routef "/todos/%s"
                      (fun id next context -> text ("Delete " + id) next context) ]

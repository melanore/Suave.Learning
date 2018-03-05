namespace SuaveRestApi

open System.Collections.Generic
type Person = {
    Id : int
    Name : string
    Age : int
    Email : string
}

module Db =
    let private peopleStorage = Dictionary<int, Person>()
    let getPeople () = 
        peopleStorage.Values :> seq<Person>
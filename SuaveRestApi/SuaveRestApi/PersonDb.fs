namespace SuaveRestApi

open System.Collections.Generic
type Person = {
    Id : int
    Name : string
    Age : int
    Email : string
}

module PersonDb =
    let private peopleStorage = Dictionary<int, Person>()
    let getPeople () = peopleStorage.Values :> seq<Person>
    let personExists = peopleStorage.ContainsKey

    let getPersonById personId = 
        if peopleStorage.ContainsKey(personId) 
        then Some peopleStorage.[personId] 
        else None 

    let createPerson person = 
        let id = peopleStorage.Values.Count + 1
        let newPerson = { person with Id = id }
        peopleStorage.Add(id, newPerson)
        newPerson

    let updatePersonById personId personToBeUpdated = 
        if peopleStorage.ContainsKey(personId) then
            let updatedPerson = { personToBeUpdated with Id = personId }
            peopleStorage.[personId] <- updatedPerson
            Some updatedPerson
        else 
            None
        
    let updatePerson personToBeUpdated = 
        updatePersonById personToBeUpdated.Id personToBeUpdated

    let deletePerson personId = 
        match peopleStorage.Remove personId with
        | true  -> Some ()
        | _     -> None 
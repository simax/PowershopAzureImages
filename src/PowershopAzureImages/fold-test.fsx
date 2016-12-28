open System

type Rule = string -> bool * string

let rules : Rule list = 
    [fun text -> printfn "Rule 1 " 
                 (text.Split ' ').Length = 3, "Must be 3 words"
     fun text -> printfn "Rule 2 " 
                 text.Length <= 30, "Max length 30 characters"
     fun text -> printfn "Rule 3 " 
                 text.ToCharArray()
                 |> Array.filter Char.IsLetter
                 |> Array.forall Char.IsUpper, "Must all be caps" ]
                 

let reducer = fun firstRule secondRule word ->
                    //printfn "word: %s" word    
                    let passed, error = firstRule word
                    if passed then
                        let passed, error = secondRule word
                        if passed then true, "" else false, error 
                    else false, error

let buildValidator (rules : Rule list) =
    rules
    |> List.reduce reducer    

let validate = buildValidator rules                    
let phrase = "HELLO FrOM F#"

validate phrase
                      
let tryLoadCustomer id = 
    if id >= 2 && id <= 7 then Some <| sprintf "Customer %d" id else None  

let ids = 
    [1; 2; 3; 4; 5; 6; 7; 8; 9; 10]
    |> List.choose tryLoadCustomer 
    
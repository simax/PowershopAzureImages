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

let buildValidator (rules : Rule list) =
    rules
    |> List.reduce (fun firstRule secondRule word ->
                    printfn "word: %s" word    
                    let passed, error = firstRule word
                    if passed then
                        let passed, error = secondRule word
                        if passed then true, "" else false, error 
                    else false, error)    

let validate = buildValidator rules                    
let phrase = "HELLO FrOM F#"

validate phrase
                      
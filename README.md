# TruckTransmissionGenerator
TruckTransmissionGenerator is a console program which generatres truck transmission with the help of a json file. 

WIP

## Example json

```
{
    "game": "American Truck Simulator",
    "templateFolder": "",
    "outputFolder": "",
    "trucks": [
        "kenworth.t680",
        "kenworth.w900",
    ],
    "transmissions": [
        {
            "name": "allison.4500",
            "displayName": "Allison 4500",
            "price": 11990,
            "unlock": 0,
            "retarder": false
        },
        {
            "name": "allison.4500_r",
            "displayName": "Allison 4500 R",
            "price": 12490,
            "unlock": 0,
            "retarder": true
        }
    ],
    "differential_ratios": [ "4.10", "3.42" ]
}
```
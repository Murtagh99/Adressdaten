function getAddress(indicator) {
    return new Promise(resolve => {
        $.ajax({
            type: "GET",
            url: readInput(indicator),
            cache: false,
            success: function (data) {
                console.log(data);
                resolve(data);
            }
        })
    })

}

function readInput(indicator) {
    let city = document.getElementById("inputcities").value;
    let street = document.getElementById("inputstreets").value;
    let matching = city.match(/^\d+/);
    let url = "api/adressdaten";

    if (indicator && matching[0].length === 5) {
        url = url.concat("/Street/postcode=" + matching[0]);
        if (street) {
            url = url.concat("/street=" + street.toLowerCase());
        }
    } else if (!indicator) {
        url = url.concat("/City");
        if (matching) {
            url = url.concat("/postcode=" + matching[0])
        } else if (city) {
            url = url.concat("/name=" + city.toLowerCase())
        }
    } else {
        url = url.concat("/norequest")
    }
    return url;
}

async function getOptions(indicator, listId) {
    let AdressItemArray = await getAddress(indicator);
    const list = document.getElementById(listId);
    const oldItemsLength = list.childElementCount;
    if (!list.firstChild || !(document.getElementById("input" + listId).innerHTML === list.firstChild.innerHTML)) {
        if (indicator) {
            AdressItemArray.forEach(Item => list.appendChild(document.createElement("option")).setAttribute("value", Item.name));
        } else {
            AdressItemArray.forEach(Item => list.appendChild(document.createElement("option")).setAttribute("value", Item.postCode + " " + Item.name));
        }
        for (i = 0; i < oldItemsLength; i++) {
            list.removeChild(list.firstChild);
        }
    }
}
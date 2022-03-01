
function InfoCardUpdate(result)
{
    document.getElementById("WorkflowInfoId").innerHTML = result.info;
    let sideOverview = document.getElementById("OverviewDivId").children[0];
    let div = document.createElement("div");
    div.innerHTML = result.card;
    sideOverview.children[0].innerHTML = div.children[0].innerHTML;
}
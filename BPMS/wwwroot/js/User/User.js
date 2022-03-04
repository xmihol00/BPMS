
var RemovedRoles = [];
var AddedRoles = [];

function RemoveRole(btn, role)
{
    let roleName = btn.parentNode.innerText;
    let length = AddedRoles.length;
    AddedRoles = AddedRoles.filter(x => x != role);
    if (length == AddedRoles.length)
    {
        RemovedRoles.push(role);
    }

    let div = document.createElement("div");
    div.className = "badge bg-secondary rounded-pill text-default input-badge";
    div.innerHTML = `${roleName}<i class="fas fa-plus my-auto ms-2 pointer" onclick="AddRole(this, '${role}')"></i>`;
    btn.parentNode.parentNode.append(div);
    btn.parentNode.remove();
}

function AddRole(btn, role)
{
    let roleName = btn.parentNode.innerText;
    let length = RemovedRoles.length;
    RemovedRoles = RemovedRoles.filter(x => x != role);
    if (length == RemovedRoles.length)
    {
        AddedRoles.push(role);
    }

    let div = document.createElement("div");
    div.className = "badge bg-primary rounded-pill text-default input-badge";
    div.innerHTML = `${roleName}<i class="fas fa-times my-auto ms-2 pointer" onclick="RemoveRole(this, '${role}')"></i>`;
    btn.parentNode.parentNode.prepend(div);
    btn.parentNode.remove();
}

function AddRoleInputs()
{
    let form = document.getElementById("CreateEditUserId");
    for (let i = 0; i < AddedRoles.length; i++) 
    {
        CreateInput(AddedRoles[i], `AddedRoles[${i}]`, form);
    }

    for (let i = 0; i < RemovedRoles.length; i++) 
    {
        CreateInput(RemovedRoles[i], `RemovedRoles[${i}]`, form);
    }

    AddedRoles = [];
    RemovedRoles = [];
}


function RemoveRoleInpust()
{
    for (let ele of document.querySelectorAll("[data-gen]"))
    {
        ele.remove();
    }   
}

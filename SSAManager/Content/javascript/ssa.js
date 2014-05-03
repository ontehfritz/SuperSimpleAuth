$("input[name='claim']").change(function() {
    $("#Claims").val($("input[name='claim']").map(function() {
        if(this.checked)
        {
            return $(this).val();
        }
    })
    .get()
    .join());
});

$("input[name='user']").change(function() {
    $("#RoleUsers").val($("input[name='user']").map(function() {
        if(this.checked)
        {
            return $(this).val();
        }
    })
    .get()
    .join());
    
     $("#uRoleUsers").val($("input[name='user']").map(function() {
        if(!this.checked)
        {
            return $(this).val();
        }
    })
    .get()
    .join());
});

$("input[name='role']").change(function() {
    $("#Roles").val($("input[name='role']").map(function() {
        if(this.checked)
        {
            return $(this).val();
        }
    })
    .get()
    .join());
});


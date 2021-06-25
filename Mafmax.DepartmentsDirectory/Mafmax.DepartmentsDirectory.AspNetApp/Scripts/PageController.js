var session = $.connection.sessionHub;
$(function () {
    session.client.onMessage = function (message) {
        alert(message);
    }
    session.client.onDownloadXml = function (fileName, data) {
        console.log('after onDownload invocation');
        download(data, fileName, 'text/xml');

    }
    session.client.onEditDepartment = function (department) {
        $('#department-name' + department.Id).text(department.Name + ' [' + department.Id + ']');
    }
    session.client.onRemoveDepartment = function (department) {
        $('#department' + department.Id).remove();
    }
    session.client.onAddDepartment = function (department) {
        addDepartment(department);
    }
    session.client.onAddGroup = function (group, departmentId) {
        addGroup(group, departmentId);
    }
    session.client.onReplaceGroup = function (group, departmentId) {
        $('#group' + group.Id).remove();
        addGroup(group, departmentId);
    }
    session.client.onRemoveGroup = function (id) {
        $('#group' + id).remove();
    };
    session.client.onEditGroup = function (group) {
        
        $('#group-name' + group.Id).text(group.Name + ' [' + group.Id + ']');
    }

    session.client.onCompanyChanged = function (company) {
        changeCompanyItem(company.Id, company.Name);
    }
    session.client.onCurrentCompanyChanged = function (company) {
        changeCurrentCompany(company);

    }

    session.client.onCompanyRemoved = function (company) {
        removeCompanyItem(company.Id);
    }
    session.client.onCurrentCompanyRemoved = function () {
        exitCompanyProcessingPage();
        session.server.onStartPage();
    }
    session.client.onCompanyAdded = function (company) {
        addCompanyItem(company.Name, company.Id);
    }
    session.client.onStartPage = function (companies) {
        start(companies);
    }
    session.client.onCompanyProcessingPage = function (company) {
        startProcessingCompany(company);
    }
    $.connection.hub.start().done(function () {
        session.server.onStartPage();

    });

})
function exitAllPages() {
    exitStartPage();
    exitCompanyProcessingPage();
}
function exitStartPage() {
    $('.container-startPage').remove();
}
function exitCompanyProcessingPage() {
    $('.container-companyProcessingPage').remove();
}
function removeCompanyItem(id) {
    $('#company' + id).remove();
}
function changeCompanyItem(id, newName) {
    $('#company-name' + id).text(newName + ' [' + id + ']');
}
function changeCurrentCompany(company) {

    $('#company-name').text('Текущая компания: ' + company.Name + ' [' + company.Id + ']');
}
function addCompanyItem(name, id) {

    var nameBlock = $('<div/>').text(name + ' [' + id + ']').attr('id', 'company-name' + id);
    var edit = getEditButton('Изменить название компании');
    var remove = getRemoveButton('Удалить компанию');
    var signin = getSignInButton("Начать работу с компанией");

    remove.on('click', function () {
        if (confirm('Вы действительно хотите удалить компанию: ' + name + '?\nБудут удалены все департаменты и отделы, входящие в эту компанию.')) {
            session.server.removeCompany(id);
        }
    });
    edit.on('click', function () {
        var newName = prompt('Введите название компании.', name);
        if (newName != null) {
            session.server.changeCompany(id, newName);
        }
    });
    signin.on('click', function () {
        session.server.startProcessingCompany(id);
    });

    var buttons = $('<div/>').append(edit).append(remove).append(signin);
    buttons.hide();
    var itemPanel = $('<div/>').append(nameBlock).append(buttons);
    var li = $('<li/>').append(itemPanel).attr('id', 'company' + id);
    nameBlock.addClass('nameblock');
    buttons.addClass('buttons');
    subscribeMouseOverEvents(itemPanel);
    $('#company-list').append(li);
}
function start(companies) {
    exitAllPages();
    var list = $('<ol/>').attr('id', 'company-list');
    var label = $('<div/>').text('Выберите компанию из списка или создайте её, чтобы начать работу.');
    var addLabel = $('<div/>').text('Добавить новую компанию ');
    var uploadLabel = $('<div/>').text('Выберите XML файл для загрузки справочника подразделений: ');
    var uploadFile = $('<input/>').attr('type', 'file').attr('accept', 'text/xml');
    var downloadLabel = $('<div/>').text('Скачать данные в формате XML: ');
    var downloadFile = $('<button/>').append($('<span/>').addClass('fa').addClass('fa-download'));
    addLabel.css('margin-top', '10px');
    var btn = getAddButton('Новая компания');
    btn.on('click', function () {
        var newName = prompt('Введите название компании.', '');
        if (newName != null) {
            session.server.addCompany(newName);
        }
    });
    downloadFile.on('click', function () {
        console.clear();
        console.log('download button clicking');
        session.server.downloadXml();
        console.log('after server downloadXML method invoke');

    });
    uploadFile.on('change', function () {
        var file = this.files[0];
        const reader = new FileReader();
        reader.addEventListener('load', (event) => {
            var data = event.target.result;
            session.server.uploadXml(data);

        });
        reader.readAsText(file);
    });
    addLabel.append(btn);
    downloadLabel.append(downloadFile);
    uploadLabel.append(uploadFile);
    label.append(addLabel).append(uploadLabel).append(downloadLabel);
    var companiesList = $('<div/>').append(label).append(list).addClass('container-startPage');
    $('.container-startPage').append(companiesList);
    $('body').prepend(companiesList);
    for (var company of companies) {
        addCompanyItem(company.Name, company.Id);
    }

}
function download(data, filename, type) {
    console.log('start download');
    var file = new Blob([data], { type: type });
    console.log('after file create');
    url = URL.createObjectURL(file);
    console.log('after url create');
    var a = document.createElement("a");
    console.log('after create a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    console.log('after append a');
    a.click();
    console.log('after click a');
    setTimeout(function () {
        console.log('in setTimeoit func');
        document.body.removeChild(a);
        console.log('after remove a');
        window.URL.revokeObjectURL(url);
        console.log('after revokeURL');
    }, 0);

}
function startProcessingCompany(company) {
    exitAllPages();
    var inp = $('<input/>').attr('type', 'hidden').attr('id', 'processingCompanyId').val(company.Id);
    var container = $('<div/>')
    container.append(inp).addClass('container-companyProcessingPage');
    $('body').prepend(container);

    let data = company.Departments;
    addCompanyRoot(company);
    for (var i = 0; i < data.length; i++) {
        addDepartment(data[i]);
    }

    console.log('after all elements added');
}
function subscribeMouseOverEvents(element, nameBlockClass = 'nameblock', buttonsBlockClass = 'buttons') {

    /* if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|BB|PlayBook|IEMobile|Windows Phone|Kindle|Silk|Opera Mini/i.test(navigator.userAgent)) {
 
         alert("Вы используете мобильное устройство (телефон или планшет).")
 
     } else alert("Вы используете ПК.")*/
    element.on('mouseover', function () {
        $(element.find('.' + nameBlockClass)).css('float', 'left');
        $(element).css('background', '#ffe3eb');
        $(element.find('.' + buttonsBlockClass)).show();
    });
    element.on('mouseleave', function () {
        $(element.find('.' + nameBlockClass)).css('float', 'none');
        var baseColor = element.attr('baseColor');
        if (baseColor === undefined) {
            baseColor = "white";
        }
        $(element).css('background', baseColor);
        $(element.find('.' + buttonsBlockClass)).hide();
    });
}
function getSignInButton(content = "Войти") {
    var editContent = $('<span/>').addClass('fas').addClass('fa-sign-in-alt');
    return $('<button/>').append(editContent).attr('title', content).css('margin-left', '5px').css('color', '#36bab8');
}
function getBackButton(content = "Назад") {
    var editContent = $('<span/>').addClass('fas').addClass('fa-backspace');
    return $('<button/>').append(editContent).attr('title', content).css('margin-left', '5px').css('color', '#36bab8');
}

function getCollapseButton(content = "Свернуть/развернуть") {
    var editContent = $('<span/>').addClass('fas').addClass('fa-caret-down');
    return $('<span/>').append(editContent).attr('title', content).css('margin-right', '5px');
}
function getEditButton(content = "Изменить") {
    var editContent = $('<span/>').addClass('fas').addClass('fa-edit');
    return $('<button/>').append(editContent).attr('title', content).css('margin-left', '5px').css('color', '#d99134');
}
function getReplaceButton(content = "Переместить") {
    var replaceContent = $('<span/>').addClass('fas').addClass('fa-arrows-alt-v');
    return $('<button/>').append(replaceContent).attr('title', content).css('margin-left', '5px').css('color', 'blue');;
}
function getRemoveButton(content = "Удалить") {
    var removeContent = $('<span/>').addClass('fas').addClass('fa-trash');
    return $('<button/>').append(removeContent).attr('title', content).css('margin-left', '5px').css('color', 'red');;

}
function getAddButton(content = "Добавить") {

    var span = $('<span/>').addClass('fas').addClass('fa-plus-square');
    return $('<button/>').prepend(span).attr('title', content).css('margin-left', '5px').css('color', 'green');;

}

function addCompanyRoot(company) {

    var companyName = $('<div/>').text('Текущая компания: ' + company.Name + ' [' + company.Id + ']').attr('id', 'company-name');
    companyName.css('font-size', '25px')
    var addLabel = $('<div/>').text("Добавить департамент ");
    addLabel.css('margin-top', '10px');
    var backLabel = $('<div/>').text("Вернуться на главную страницу ");
    backLabel.css('margin-top', '10px');
    var ol = $('<ol/>');
    ol.attr('id', 'company-root');
    var backBtn = getBackButton('Вернуться на главную страницу');
    var addBtn = getAddButton('Новый департамент');
    backBtn.on('click', function () {
        session.server.onStartPage();
    });
    addBtn.on('click', function () {
        var name = prompt('Введите имя департамента.', '');
        session.server.addDepartment(company.Id, name);
    });
    backLabel.append(backBtn);
    addLabel.append(addBtn);
    $('.container-companyProcessingPage').append(companyName).append(backLabel).append(addLabel).append(ol);

}
function addDepartment(department) {

    var nameBlock = $('<div/>').text(department.Name + ' [' + department.Id + ']').attr('id', 'department-name' + department.Id);
    nameBlock.css('clear', 'both');
    var edit = getEditButton("Изменить название департамента");
    var replace = getReplaceButton("Переместить департамент в другую компанию");
    var remove = getRemoveButton("Удалить департамент");
    var addBtn = getAddButton("Новый отдел");
    var collapse = getCollapseButton();
    var buttons = $('<div/>').append(edit).append(replace).append(remove).append(addBtn);
    buttons.hide();
    nameBlock.prepend(collapse);
    var elementPanel = $('<div/>').append(nameBlock).append(buttons);
    var list = $('<ol/>');
    list.attr('id', 'department-list' + department.Id);
    //collapse.css('float', 'left');
    nameBlock.addClass('nameblock');
    buttons.addClass('buttons');
    var li = $('<li/>').append(elementPanel).append(list).attr('id', 'department' + department.Id);
    elementPanel.css('height', '20px');
    subscribeMouseOverEvents(elementPanel);
    elementPanel.attr('baseColor', '#bfdcff');
    elementPanel.css('background', '#bfdcff');
    $('#company-root').append(li);

    collapse.on('click', function () {
        var collapsedClass = 'fa-caret-right';
        var uncollapsedClass = 'fa-caret-down';
        var css = list.css('display');
        var collapsed = css == 'none';
        var span = collapse.find('.fas');
        if (collapsed) {

            span.addClass(collapsedClass);
            span.removeClass(uncollapsedClass);
            collapse.attr('title', "Свернуть информацию об отделах");
        } else {
            span.addClass(uncollapsedClass);
            span.removeClass(collapsedClass);
            collapse.attr('title', "Развернуть информацию об отделах");
        }
        list.toggle();
    });
    collapse.click();
    addBtn.on('click', function () {
        var name = prompt('Введите имя отдела.', 'name');
        if (name != null) {
            session.server.addGroup(department.Id, name);
        }
    });
    replace.on('click', function () {
        var number = prompt('Введите номер компании, в которую Вы хотите перенести департамент');
        if (number != null) {
            session.server.replaceDepartment(department.Id, number);
        }
    });
    edit.on('click', function () {
        var newName = prompt('Введите новое название департамента', department.Name);
        if (newName != null) {
            session.server.editDepartment(department.Id, newName);
        }
    });
    remove.on('click', function () {
        if (confirm('Вы действительно хотите удалить департамент: ' + department.Name + '?\nБудут удалены все отделы, входящие в этот департамент.')) {
            session.server.removeDepartment(department.Id);
        }
    });
    for (var i = 0; i < department.Groups.length; i++) {
        addGroup(department.Groups[i], department.Id);
    }
}

function addGroup(group, rootId) {
    var nameBlock = $('<div/>').text(group.Name + ' [' + group.Id + ']').attr('id', 'group-name' + group.Id);
    nameBlock.css('clear', 'both');
    var edit = getEditButton("Изменить название отдела");
    var replace = getReplaceButton("Переместить отдел в другой департамент");
    var remove = getRemoveButton("Удалить отдел");


    var buttons = $('<div/>').append(edit).append(replace).append(remove);
    buttons.hide();
    var li = $('<li/>').append(nameBlock).append(buttons).attr('id', 'group' + group.Id);
    nameBlock.addClass('nameblock');
    buttons.addClass('buttons');
    subscribeMouseOverEvents(li);
    li.css('height', '20px');
    li.attr('baseColor', '#e3f0ff');
    li.css('background', '#e3f0ff');
    $('#department-list' + rootId).append(li);

    replace.on('click', function () {
        var number = prompt('Введите идентификатор департамента для перемещения');
        if (number != null) {
            session.server.replaceGroup(group.Id, number);
        }
    });
    edit.on('click', function () {
        var newName = prompt('Введите новое название отдела', group.Name);
        if (newName != null) {
            session.server.editGroup(group.Id, newName);
        }
    });
    remove.on('click', function () {
        if (confirm('Вы действительно хотите удалить отдел: ' + group.Name + '?')) {

            session.server.removeGroup(group.Id);
        }
    });
}

import Vue from "vue";

const createElement = new Vue().$createElement;

const makeElement = function(type, classes, content) {
    const node = createElement(type, { class: [...classes] }, content);
    return node;
}

const makeTitleNode = function(title, classes) {
    const titleNode = makeElement('strong', classes, title);
    return titleNode;
}

const makeMessageNode = function(message, classes) {
    const messageNode = makeElement('p', classes, message);
    return messageNode;
}

const makeListNode = function(list, classes) {
    const listElements = [];

    for (let item of list) {
        let listItem = makeElement('li', classes, item);
        listElements.push(listItem);
    }

    const listNode = makeElement('ul', classes, listElements);
    return listNode;
}

const makeToast = function(title, message, list, options = {}) {
    const titleNode = makeTitleNode(title, options.titleClasses || []);
    const messageNode = makeMessageNode(message, options.messageClasses || []);
    
    let bodyNode;
    if (list != null) {
        const listNode = makeListNode(list, options.listClasses || []);
        bodyNode = makeElement('div', options.bodyClasses || [], [messageNode, listNode]);
    } else {
        bodyNode = makeElement('div', options.bodyClasses || [], [messageNode]);
    }

    const vm = new Vue();
    vm.$bvToast.toast(bodyNode, {
        title: titleNode,
        variant: options.variant || 'primary'
    });
}

export {
    makeToast
}
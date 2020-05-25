import Vue from "vue";

const BOTTOM_RIGHT = "b-toaster-bottom-right";
const CENTER_TOP = "b-toaster-center-top";

const ERROR = "danger";
const SUCCESS = "success";
const WARNING = "warning";

const successToast = function(title, message) {
    makeToast(title, message, SUCCESS, BOTTOM_RIGHT)
}

const errorToast = function(title, message) {
    makeToast(title, message, ERROR, CENTER_TOP);
}

const warningToast = function(title, message) {
    makeToast(title, message, WARNING, BOTTOM_RIGHT);
}

const makeToast = function(title, message, variant, location) {
    new Vue().$bvToast.toast(message, {
        title: title,
        variant: variant,
        autoHideDelay: 3000,
        toaster: location
    });
}

export {
    successToast,
    errorToast,
    warningToast
}


// import Vue from "vue";

// const createElement = new Vue().$createElement;

// const makeElement = function(type, classes, content) {
//     const node = createElement(type, { class: [...classes] }, content);
//     return node;
// }

// const makeTitleNode = function(title, classes) {
//     const titleNode = makeElement('strong', classes, title);
//     return titleNode;
// }

// const makeMessageNode = function(message, classes) {
//     const messageNode = makeElement('p', classes, message);
//     return messageNode;
// }

// const makeListNode = function(list, classes) {
//     const listElements = [];

//     for (let item of list) {
//         let listItem = makeElement('li', classes, item);
//         listElements.push(listItem);
//     }

//     const listNode = makeElement('ul', classes, listElements);
//     return listNode;
// }

// const makeToast = function(title, message, list, options = {}) {
//     const titleNode = makeTitleNode(title, options.titleClasses || []);
//     const messageNode = makeMessageNode(message, options.messageClasses || []);
    
//     let bodyNode;
//     if (list != null) {
//         const listNode = makeListNode(list, options.listClasses || []);
//         bodyNode = makeElement('div', options.bodyClasses || [], [messageNode, listNode]);
//     } else {
//         bodyNode = makeElement('div', options.bodyClasses || [], [messageNode]);
//     }

//     const vm = new Vue();
//     vm.$bvToast.toast(bodyNode, {
//         title: titleNode,
//         variant: options.variant || 'primary'
//     });
// }

// export {
//     makeToast
// }
import * as React from 'react';
import 'isomorphic-fetch';

export class HelperFunctions {

    // transforms date from iso formatted Date type to a string formatted according to LV standard
    static formatDate(isoDate: Date) {
        var date = new Date(isoDate);
        var result =
            ('0' + date.getUTCDate().toString()).slice(-2) + '.' +
            ('0' + (date.getMonth() + 1).toString()).slice(-2) + '.' +
            date.getFullYear().toString() + ". " +
            ('0' + date.getHours().toString()).slice(-2) + ':' +
            ('0' + date.getMinutes().toString()).slice(-2) + ':' +
            ('0' + date.getSeconds().toString()).slice(-2);
        return result;
    }
}
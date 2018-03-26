import * as React from 'react';
import 'isomorphic-fetch';
import { Credentials, Board, Value, JiraConnectionLogEntry, BoardPresentation } from './Interfaces';

export class ApiClient {
    static tokenName = 'JwtToken';

    // Redirects if response has a specified status code
    static redirect(response, statusCode, redirectLink) {
        if (response.status == statusCode) {
            open(redirectLink, '_self');
        }
        else if (!response.ok) {
            throw Error(response.statusText);
        }
        return response;
    }

    // AccountController: Checks the credentials submitted by user.
    static login(credentials: Credentials) : Promise<boolean> {
        return fetch('/api/account/login', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'authorization': 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName),
            },
            body: JSON.stringify(credentials)
        })
            .then(response => {
                if (response.ok) {
                    response.json().then(json => {
                        sessionStorage.setItem(ApiClient.tokenName, json.token);
                    });
                    return true;
                }
                else {
                    return false;
                }
            })
    }

    // AccountController: Checks if the current user has a valid JWT token
    static hasValidJwt() : Promise<Response> {
        return fetch('api/account/hasValidJwt', {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName)
            }
        })
    }

    // SampleDataController: Gets board list
    static boardList(credentials: Credentials) : Promise<Value[]> {
        return fetch('/api/SampleData/BoardList', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'authorization': 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName),
            },
            body: JSON.stringify(credentials)
        })
            .then(response => ApiClient.redirect(response, 401, './login'))
            .then(response => response.json() as Promise<Value[]>)
    }

    // SampleDataController: Gets board data
    static boardData(id: number, credentials: Credentials): Promise<Board> {
        return fetch('api/SampleData/BoardData?id=' + id.toString(), {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'authorization': 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName),
            },
            body: JSON.stringify(credentials)
        })
            .then(response => response.json() as Promise<Board>)
    }

    // SampleDataController: Increases the statistics counter each time the board is shown.
    static incrementTimesShown(id: number) {
        fetch('api/SampleData/IncrementTimesShown', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName)
            },
            body: JSON.stringify(id),
        })
    }

    // SampleDataController: Reads connection log from the appropriate file into an object.
    static networkStatistics(id: number) : Promise<JiraConnectionLogEntry[]> {
        return fetch('api/SampleData/NetworkStatistics?id=' + id.toString(), {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName)
            }
        })
            .then(response => ApiClient.redirect(response, 401, './login'))
            .then(response => response.json() as Promise<JiraConnectionLogEntry[]>)
    }

    // PresentationsController
    static getPresentations(): Promise<BoardPresentation[]> {
        return fetch('api/admin/presentations', {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName)
            }
        })
            .then(response => ApiClient.redirect(response, 401, './login'))
            .then(response => response.json())
    }

    // PresentationsController
    static getAPresentation(id: string) : Promise<BoardPresentation> {
        return fetch('api/admin/presentations/' + id, {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName)
            }
        })
            .then(response => ApiClient.redirect(response, 401, './login'))
            .then(response => response.json())
    }

    // PresentationsController
    static savePresentation(boardPresentation: BoardPresentation): Promise<BoardPresentation> {
        return fetch('api/admin/presentations/', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName)
            },
            body: JSON.stringify(boardPresentation)
        })
            .then(response => response.json())
    }

    // VersionController
    static getVersion() : Promise<string> {
        return fetch('api/version')
            .then(res => res.json() as Promise<string>)
    }
}
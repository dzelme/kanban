import * as React from 'react';
import 'isomorphic-fetch';
import { Credentials, Board, Value, JiraConnectionLogEntry, BoardPresentation } from './Interfaces';

export class ApiClient {
    static tokenName = 'JwtToken';

    // helper post method
    static post(link, data): Promise<Response> {
        return fetch(link, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'authorization': 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName),
            },
            body: JSON.stringify(data)
        })
    }

    // helper get method
    static get(link): Promise<Response> {
        return fetch(link, {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName)
            }
        })
    }

    // helper redirects if response has a specified status code
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
    static login(credentials: Credentials): Promise<boolean> {
        return ApiClient.post('/api/account/login', credentials)
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
    static hasValidJwt(): Promise<Response> {
        return ApiClient.get('api/account/hasValidJwt');
    }

    // SampleDataController: Gets board list
    static boardList(credentials: Credentials): Promise<Value[]> {
        return ApiClient.post('/api/SampleData/BoardList', credentials)
            .then(response => ApiClient.redirect(response, 401, './login'))
            .then(response => response.json() as Promise<Value[]>)
    }

    // SampleDataController: Gets board data
    static boardData(id: number, credentials: Credentials): Promise<Board> {
        return ApiClient.post('api/SampleData/BoardData?id=' + id.toString(), credentials)
            .then(response => response.json() as Promise<Board>)
    }

    // SampleDataController: Increases the statistics counter each time the board is shown.
    static incrementTimesShown(id: number) {
        return ApiClient.post('api/SampleData/IncrementTimesShown', id)
    }

    // SampleDataController: Reads connection log from the appropriate file into an object.
    static networkStatistics(id: number): Promise<JiraConnectionLogEntry[]> {
        return ApiClient.get('api/SampleData/NetworkStatistics?id=' + id.toString())
            .then(response => ApiClient.redirect(response, 401, './login'))
            .then(response => response.json() as Promise<JiraConnectionLogEntry[]>)
    }

    // PresentationsController
    static getPresentations(): Promise<BoardPresentation[]> {
        return ApiClient.get('api/admin/presentations')
            .then(response => ApiClient.redirect(response, 401, './login'))
            .then(response => response.json() as Promise<BoardPresentation[]>)
    }

    // PresentationsController
    static getAPresentation(id: string): Promise<BoardPresentation> {
        return ApiClient.get('api/admin/presentations/' + id)
            .then(response => ApiClient.redirect(response, 400, './login'))  //NB! 400
            .then(response => response.json() as Promise<BoardPresentation>)
    }

    // PresentationsController
    static savePresentation(boardPresentation: BoardPresentation): Promise<BoardPresentation> {
        return ApiClient.post('api/admin/presentations/', boardPresentation)
            .then(response => response.json() as Promise<BoardPresentation>)
    }

    // VersionController
    static getVersion(): Promise<string> {
        return fetch('api/version')
            .then(response => response.json() as Promise<string>)
    }
}
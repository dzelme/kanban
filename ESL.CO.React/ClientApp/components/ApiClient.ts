import * as React from 'react';
import 'isomorphic-fetch';
import { Credentials, Board, Value, JiraConnectionLogEntry, BoardPresentation } from './Interfaces';

function handleResponse(response: Response): Promise<any> {
    if (response.ok) return response.json();

    if (response.status == 401) {
        window.open('./login', '_self');
    }

    console.error(response);
    throw Error(response.statusText);
}

export class ApiClient {

    static tokenName = 'JwtToken';

    // helper post method
    static post(link, data): Promise<any> {
        return fetch(link, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'authorization': 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName),
            },
            body: JSON.stringify(data)
        }).then(handleResponse);
    }

    // helper get method
    static get(link): Promise<any> {
        return fetch(link, {
            headers: {
                'Accept': 'application/json',
                authorization: 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName)
            }
        })
        .then(handleResponse);
    }

    // helper delete method
    static delete(link) {
        return fetch(link, {
            method: 'DELETE',
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName)
            }
        })
        .then(handleResponse);
    }

    // helper redirects if response has a specified status code
    // static redirect(response, statusCode, redirectLink) {
    //     if (response.status == statusCode) {
    //         open(redirectLink, '_self');
    //     }
    //     else if (!response.ok) {
    //         throw Error(response.statusText);
    //     }
    //     return response;
    // }

    // AccountController: Checks the credentials submitted by user.
    static login(credentials: Credentials): Promise<boolean> {
        return ApiClient.post('api/account/login', credentials)
            .then(json => {
                sessionStorage.setItem(ApiClient.tokenName, json.token);

                // if (response.ok) {
                //     response.json().then(json => {
                //         sessionStorage.setItem(ApiClient.tokenName, json.token);
                //     });
                //     return true;
                // }
                // else {
                //     return false;
                // }
                return true;
            })
            .catch(err => {
                return false;
            });
    }

    // AccountController: Checks if the current user has a valid JWT token
    static hasValidJwt(redirect = true ): Promise<boolean> {
        var p = fetch('api/account/hasValidJwt');
        if (redirect) {
            return p.then(handleResponse).then(() => true);
        }

        return p.then(response => response.ok);
    }

    // SampleDataController: Gets board list
    static boardList(credentials: Credentials): Promise<Value[]> {
        return ApiClient.post('api/SampleData/BoardList', credentials) as Promise<Value[]>;
    }

    // SampleDataController: Gets board data
    static boardData(id: number, credentials: Credentials): Promise<Board> {
        return ApiClient.post('api/SampleData/BoardData?id=' + id.toString(), credentials) as Promise<Board>;
    }

    // SampleDataController: Increases the statistics counter each time the board is shown.
    static incrementTimesShown(id: number) {
        return ApiClient.post('api/SampleData/IncrementTimesShown', id)
    }

    // SampleDataController: Reads connection log from the appropriate file into an object.
    static networkStatistics(id: number): Promise<JiraConnectionLogEntry[]> {
        return ApiClient.get('api/SampleData/NetworkStatistics?id=' + id.toString()) as Promise<JiraConnectionLogEntry[]>;
    }

    // PresentationsController
    static getPresentations(): Promise<BoardPresentation[]> {
        return ApiClient.get('api/admin/presentations') as Promise<BoardPresentation[]>;
    }

    // PresentationsController
    static getAPresentation(id: string): Promise<BoardPresentation> {
        return ApiClient.get('api/admin/presentations/' + id) as Promise<BoardPresentation>;
    }

    // PresentationsController
    static savePresentation(boardPresentation: BoardPresentation): Promise<BoardPresentation> {
        return ApiClient.post('api/admin/presentations/', boardPresentation) as Promise<BoardPresentation>;
    }

    // PresentationsController
    static deletePresentation(id: string) {
        ApiClient.delete('api/admin/presentations/' + id)
    }

    // VersionController
    static getVersion(): Promise<string> {
        return ApiClient.get('api/version') as Promise<string>;
    }
}
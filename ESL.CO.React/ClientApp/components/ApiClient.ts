import * as React from 'react';
import 'isomorphic-fetch';
import { Credentials } from './Interfaces';

//export class ApiClient {
//    login(username: string, password: string) : Promise<string> {
//        return fetch('/api/login', {
//            data: {
//                username,
//                password
//            }
//        }).then(r => r.json())
//            .then(x => something())
//            .then()..;

//    }
//}


export class ApiClient {
    private tokenName = 'JwtToken'; //?

    static login(credentials: Credentials) : Promise<boolean> { // : Promise<string> {
        return fetch('/api/account/login', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'authorization': 'Bearer ' + sessionStorage.getItem('JwtToken'),
            },
            body: JSON.stringify(credentials)
        })
            .then(response => {
                if (response.ok) {
                    response.json().then(json => {
                        sessionStorage.setItem('JwtToken', json.token);
                    });
                    return true;
                    //this.setState({ invalidCredentials: false });
                    //open('./admin/presentations', '_self');
                }
                else {
                    //this.setState({ invalidCredentials: true });
                    return false;
                }
            })
    }

    static checkCredentials() : Promise<Response> {
        return fetch('api/account/checkcredentials', {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
            }
        })
    }

    static redirect(response, statusCode, redirectLink) {
        if (response.status == statusCode) {
            open(redirectLink, '_self');
        }
        else if (!response.ok) {
            throw Error(response.statusText);
        }
        return response;
    }

    static boardList(credentials: Credentials) : Promise<Response> {
        return fetch('/api/SampleData/BoardList', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'authorization': 'Bearer ' + sessionStorage.getItem('JwtToken'),
            },
            body: JSON.stringify(credentials)
        })
            .then(response => ApiClient.redirect(response, 401, './login'))
    }
}










/*
const withFetching = (url) => (Comp) =>
    class WithFetching extends React.Component {
        constructor(props) {
            super(props);

            this.state = {
                data: {},
                isLoading: false,
                error: null,
            };
        }

        componentDidMount() {
            this.setState({ isLoading: true });

            fetch(url)
                .then(response => {
                    if (response.ok) {
                        return response.json();
                    } else {
                        throw new Error('Something went wrong ...');
                    }
                })
                .then(data => this.setState({ data, isLoading: false }))
                .catch(error => this.setState({ error, isLoading: false }));
        }

        render() {
            return <Comp { ...this.props } { ...this.state } />
        }
    }


*/











// used to redirect to login screen, if invalid JWT token
//export function checkCredentials() {

//    fetch('api/account/checkcredentials', {
//        headers: {
//            authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
//        }
//    })
//        .then(redirectIfUnauthorized)
//}

export function fetchPost(url: string, needsAuthorization: boolean, body): Promise<Response> {
    return fetch(url, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'authorization': (true) ? 'Bearer ' + sessionStorage.getItem('JwtToken') : null,
        },
        body: JSON.stringify(body),
    })
        //.then(response => {
        //    if (response.ok) {
        //        responseOk();
        //    }
        //    else {
        //        responseBad();
        //    }
        //})
}

export function fetchGet(url: string, needsAuthorization: boolean): Promise<Response> {
    return fetch(url, )
}

//    fetch('./api/account/login', {
//        method: 'POST',
//        headers: {
//            'Accept': 'application/json',
//            'Content-Type': 'application/json'
//        },
//        body: JSON.stringify(this.state.credentials),
//    })
//        .then(response => {
//            if (response.ok) {
//                response.json().then(json => {
//                    sessionStorage.setItem('JwtToken', json.token);
//                });
//                this.setState({ invalidCredentials: false });
//                open('./admin/presentations', '_self');
//            }
//            else {
//                this.setState({ invalidCredentials: true });
//            }
//        });
//}

//export class BoardList extends React.Component<RouteComponentProps<{}>, BoardListState> {


//    handleAuth() {

//        fetch('./api/account/login', {
//            method: 'POST',
//            headers: {
//                'Accept': 'application/json',
//                'Content-Type': 'application/json'
//            },
//            body: JSON.stringify(this.state.credentials),
//        })
//            .then(response => {
//                if (response.ok) {
//                    this.setState({ invalidCredentials: false }, this.handleFetch);
//                }
//                else {
//                    this.setState({ invalidCredentials: true });
//                }
//            });
//    }

//    handleForm(event) {
//        event.preventDefault();

//        var username = document.forms['presentation'].elements["username"].value;
//        var password = document.forms['presentation'].elements["password"].value;

//        this.setState({ credentials: { username: username, password: password }, loading: true }, this.handleAuth)

//    }

//    handleFetch() {

//        function handleErrors(response) {
//            if (response.status == 401) {
//                open('./login', '_self');
//            }
//            else if (!response.ok) {
//                throw Error(response.statusText);
//            }
//            return response;
//        }

//        fetch('api/SampleData/BoardList/?credentials=' + this.state.credentials.username + ":" + this.state.credentials.password, {
//            headers: {
//                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
//            }
//        })
//            .then(handleErrors)
//            .then(response => response.json() as Promise<Value[]>)
//            .then(data => {
//                this.setState({ boardPresentation: null, boardList: data, loading: false });
//            });
//    }

//    handleSubmit(event) {
//        event.preventDefault();

//        var val = new Array();

//        this.state.boardList.map(board => {
//            board.visibility = document.forms['boardlist'].elements[board.id + "visibility"].checked;
//            if (board.visibility == true) { val.push(board); }
//            board.timeShown = parseInt(document.forms['boardlist'].elements[board.id + "timeShown"].value);
//            board.refreshRate = parseInt(document.forms['boardlist'].elements[board.id + "refreshRate"].value);
//        })

//        this.setState({
//            boardPresentation: {
//                id:"",
//                title: document.forms['presentation'].elements["title"].value,
//                owner: document.forms['presentation'].elements["username"].value,
//                credentials: {
//                    username: document.forms['presentation'].elements["username"].value,
//                    password: document.forms['presentation'].elements["password"].value
//                },
//                boards: {
//                    values: val,
//                }
//            }
//        }, this.postPresentation)

//    }

//    postPresentation() {

//        fetch('api/admin/Presentations/', {
//            method: 'POST',
//            headers: {
//                'Accept': 'application/json',
//                'Content-Type': 'application/json',
//                'Authorization': 'Bearer ' + sessionStorage.getItem('JwtToken')
//            },
//            body: JSON.stringify(this.state.boardPresentation),
//        });

//        open('./admin/presentations', '_self');
//    }


import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { Credentials, Value, BoardPresentation } from './Interfaces';
import jwt_decode from 'jwt-decode';
import { ApiClient } from './ApiClient';

interface EditPresentationState {
    boardPresentation: BoardPresentation;
    boardlist: Value[];
    loading: boolean;
    credentials: Credentials;
    authenticated: boolean;
}

export class EditPresentation extends React.Component<RouteComponentProps<{ id: string }>, EditPresentationState> {
    constructor(props: RouteComponentProps<{ id: string }>) {
        super(props);

        this.state = {
            boardPresentation: {
                id: "",
                title: "",
                owner: "",
                credentials: {
                    username: "",
                    password: "",
                },
                boards: {
                    values: []
                }
            },
            boardlist: [],
            credentials: { username: "", password: "" },
            loading: true,
            authenticated: true
        };

        this.handleFetch = this.handleFetch.bind(this);
        this.handleVisibility = this.handleVisibility.bind(this);
        this.handleForm = this.handleForm.bind(this);
        this.handleAuth = this.handleAuth.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.postPresentation = this.postPresentation.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.handleChangeBoardVisibility = this.handleChangeBoardVisibility.bind(this);
        this.handleChangeBoardTimes = this.handleChangeBoardTimes.bind(this);

        ApiClient.getAPresentation(this.props.match.params.id)
            .then(data => {
                this.setState({ boardPresentation: data, credentials: data.credentials }, this.handleFetch);
            });
    }

    handleFetch() {
        ApiClient.boardList(this.state.boardPresentation.credentials)
            .then(data => {
                this.setState({ boardPresentation: this.state.boardPresentation, boardlist: data }, this.handleVisibility);
            });
    }

    handleVisibility() {
        var newList = EditPresentation.checkVisibility(this.state.boardlist, this.state.boardPresentation.boards.values);
        this.setState({ boardlist: newList, loading: false });
    }

    handleForm(event) {
        event.preventDefault();

        if (this.state.boardPresentation.credentials.username == this.state.credentials.username && this.state.boardPresentation.credentials.password == this.state.credentials.password) {
            this.setState({ loading: false });
        }
        else {
            this.setState({ boardlist: null, loading: true }, this.handleAuth)
        }
    }

    handleAuth() {
        ApiClient.login(this.state.credentials)
            .then(response => {
                if (response) {
                    this.setState({ authenticated: true }, this.handleFetch);
                }
                else {
                    this.setState({ authenticated: false });
                }
            });
    }

    handleSubmit(event) {
        event.preventDefault();

        var val = new Array();

        this.state.boardlist.map(board => {
            if (board.visibility == true) { val.push(board); }
        })

        this.setState({
            boardPresentation: {
                id: this.state.boardPresentation.id,
                title: this.state.boardPresentation.title,
                owner: this.state.boardPresentation.owner,
                credentials: this.state.credentials,
                boards: {
                    values: val,
                }
            }
        }, this.postPresentation)

    }

    postPresentation() {
        ApiClient.savePresentation(this.state.boardPresentation);
        open('./admin/presentations', '_self');
    }

    handleChange(event) {
        const target = event.target;
        const name = target.name;

        if (name == 'title') {
            this.setState({ boardPresentation: { id: this.state.boardPresentation.id, title: event.target.value, owner: this.state.boardPresentation.owner, credentials: this.state.boardPresentation.credentials, boards: this.state.boardPresentation.boards } });
        }
        else if (name == 'username') {
            this.setState({ credentials: { username: event.target.value, password: this.state.credentials.password } });
        }
        else {
            this.setState({ credentials: { username: this.state.credentials.username, password: event.target.value } });
        }
    }

    handleChangeBoardVisibility(id: string) {
        var newBoardlist = this.state.boardlist;

        this.state.boardlist.map((board, index) => {
            if (board.id.toString() == id) {
                newBoardlist[index].visibility = !newBoardlist[index].visibility

                this.setState({
                    boardlist: newBoardlist
                });
            }
        })
    }

    handleChangeBoardTimes(id: string, name: string, e) {
        var newBoardlist = this.state.boardlist;
        var value = parseInt(e.target.value);

        if (name == 'timeShown') {

            this.state.boardlist.map((board, index) => {

                if (board.id.toString() == id) {
                    newBoardlist[index].timeShown = value

                    this.setState({
                        boardlist: newBoardlist
                    });
                }
            })
        }
        else {
            this.state.boardlist.map((board, index) => {

                if (board.id.toString() == id) {
                    newBoardlist[index].refreshRate = value

                    this.setState({
                        boardlist: newBoardlist
                    });
                }
            })
        }
    }

    // used to redirect to login screen, if invalid JWT token
    componentWillMount() {
        ApiClient.hasValidJwt()
            .then(response => ApiClient.redirect(response, 401, './login'));
    }

    public render() {
        if (sessionStorage.getItem('JwtToken') === null) {
            return null;
        }

        let contents = this.state.loading
            ? null
            : EditPresentation.renderBoardList(this.state.boardlist, this.handleSubmit, this.handleChangeBoardVisibility, this.handleChangeBoardTimes);

        let error = this.state.authenticated
            ? <h4>Brīdinājums! Lietotājvārds un parole tiks glabāti atklātā tekstā uz servera!</h4>
            : <h4>Nekorekts lietotājvārds un/vai parole!</h4>


        return <div className="top-padding">

            <h1>Rediģēt prezentāciju</h1>

            <form name="presentation" onSubmit={this.handleForm}>
                <div key="title" style={styleForm}>
                    Nosaukums: <input id="title" name="title" type="text" value={this.state.boardPresentation.title} onChange={this.handleChange} />
                </div>
                <div key="owner" style={styleForm}>
                    Izveidotājs: <input id="owner" name="owner" type="text" disabled value={this.state.boardPresentation.owner} />
                </div>
                <div key="username" style={styleForm}>
                    Lietotājvārds: <input id="username" required name="username" type="text" value={this.state.credentials.username} onChange={this.handleChange} />
                </div>
                <div key="password" style={styleForm}>
                    Parole: <input id="password" required name="password" type="password" value={this.state.credentials.password} onChange={this.handleChange} />
                </div>
                <div style={styleButton}><button type="submit" className="btn btn-default">Apstiprināt izmaiņas <br /> autentifikācijas datos</button></div>
            </form>

            {error}
            {contents}
        </div>;
    }

    private static checkVisibility(allBoards: Value[], presentationBoards: Value[]) {

        presentationBoards.map(presBoard =>
            allBoards.map(board => {
                if (board.id == presBoard.id) {
                    board.visibility = true,
                        board.timeShown = presBoard.timeShown,
                        board.refreshRate = presBoard.refreshRate
                }
            })
        )
        return allBoards;
    }

    private static renderBoardList(boardList: Value[], handleSubmit, handleChangeBoardVisibility, handleChangeBoardTimes) {

        return <div>
            <form name='boardlist' onSubmit={handleSubmit}>
                <table className='table'>
                    <thead style={styleHeader}>
                        <tr>
                            <th>ID</th>
                            <th>Nosaukums</th>
                            <th>Tips</th>
                            <th style={styleCheckBoxTitle}>Iekļaut prezentācijā</th>
                            <th>Attēlošanas laiks(ms)</th>
                            <th>Atjaunošanas laiks(ms)</th>
                        </tr>
                    </thead>
                    <tbody style={styleContent}>
                        {boardList.map(board =>
                            <tr key={board.id + "row"}>
                                <td key={board.id + ""}>{board.id}</td>
                                <td key={board.id + "name"}>{board.name}</td>
                                <td key={board.id + "type"}>{board.type}</td>
                                <td key={board.id + "visibility"}><input style={styleCheckBox} name={board.id + "visibility"} type="checkbox" defaultChecked={board.visibility} onClick={() => handleChangeBoardVisibility(board.id)} /></td>
                                <td key={board.id + "timeShown"}><input name={board.id + "timeShown"} type="number" value={board.timeShown.toString()} onChange={(e) => handleChangeBoardTimes(board.id, 'timeShown', e)} /></td>
                                <td key={board.id + "refreshRate"}><input name={board.id + "refreshRate"} type="number" value={board.refreshRate.toString()} onChange={(e) => handleChangeBoardTimes(board.id, 'refreshRate', e)} /></td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <button className="btn btn-default" type="submit">Saglabāt izmaiņas</button>
            </form>
        </div>
    }
}
const styleHeader = {
    fontSize: '20px'
}

const styleContent = {
    fontSize: '15px'
}

const styleForm = {
    fontSize: '20px',
    display: 'inline-block',
    margin: '10px',
    marginBottom: '30px'
}

const styleButton = {
    display: 'inline-block',
    height: '40px'
}

const styleCheckBox = {
    marginLeft: '50%'
}

const styleCheckBoxTitle = {
    textAlign: 'center'
}
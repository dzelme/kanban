import * as React from 'react';
import 'isomorphic-fetch';
import jwt_decode from 'jwt-decode';
import { RouteComponentProps } from 'react-router';
import { EditPresentationState, Value, BoardPresentation } from './Interfaces';
import { ApiClient } from './ApiClient';

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
            boardList: [],
            loading: true,
            error: ""
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

        ApiClient.getPresentation(this.props.match.params.id)
            .then(data => {
                this.setState({ boardPresentation: data }, this.handleFetch);
            });
    }

    handleForm(event) {
        event.preventDefault();

        this.handleAuth();
    }

    handleAuth() {
        ApiClient.savePresentation(this.state.boardPresentation)
            .then(response => {
                if (response.status == 200 || response.status == 400) {
                    this.setState({ error: "" }, this.handleFetch);
                }
                else {
                    this.setState({ error: "Nekorekts lietotājvārds un/ vai parole!", loading: true });
                }
            });
    }

    handleFetch() {
        ApiClient.boardListFromId(this.state.boardPresentation.id)
            .then(data => {
                this.setState({ boardPresentation: this.state.boardPresentation, boardList: data }, this.handleVisibility);
            });
    }

    handleVisibility() {
        var newList = EditPresentation.checkVisibility(this.state.boardList, this.state.boardPresentation.boards.values);
        this.setState({ boardList: newList, loading: false });
    }

    handleSubmit(event) {
        event.preventDefault();

        var val = new Array();

        this.state.boardList.map(board => {
            if (board.visibility == true) { val.push(board); }
        })

        let presentation = {
            id: this.state.boardPresentation.id,
            title: this.state.boardPresentation.title,
            owner: this.state.boardPresentation.owner,
            credentials: this.state.boardPresentation.credentials,
            boards: {
                values: val,
            }
        }
        this.postPresentation(presentation)
    }

    postPresentation(presentation: BoardPresentation) {
        ApiClient.savePresentation(presentation)
            .then(response => {
                if (response.status == 200) {
                    open('./admin/presentations', '_self')
                }
                if (response.status == 400) {
                    this.setState({ error: "Ievadītie dati nav pareizi. Pārbaudiet vērtību pieļaujamās robežas." });
                }
            });
    }

    handleChange(event) {
        const target = event.target;
        const name = target.name;

        let newBoardPresentation = Object.create(this.state.boardPresentation);
        newBoardPresentation.id = this.state.boardPresentation.id,
        newBoardPresentation.title = this.state.boardPresentation.title,
        newBoardPresentation.owner = this.state.boardPresentation.owner,
        newBoardPresentation.credentials = this.state.boardPresentation.credentials,
        newBoardPresentation.boards = this.state.boardPresentation.boards
        

        if (name == 'title') {
            newBoardPresentation.title = event.target.value;
        }
        else if (name == 'username') {
            newBoardPresentation.credentials.username = event.target.value;
        }
        else {
            newBoardPresentation.credentials.password = event.target.value;
        }

        this.setState({ boardPresentation: newBoardPresentation });
    }

    handleChangeBoardVisibility(id: string) {
        var newBoardlist = this.state.boardList.concat();  // to clone not reference

        this.state.boardList.map((board, index) => {
            if (board.id.toString() == id) {
                newBoardlist[index].visibility = !newBoardlist[index].visibility

                this.setState({
                    boardList: newBoardlist
                });
            }
        })
    }

    handleChangeBoardTimes(id: string, name: string, e) {
        var newBoardList = this.state.boardList.concat();
        var value = parseInt(e.target.value);

        if (name == 'timeShown') {

            this.state.boardList.map((board, index) => {

                if (board.id.toString() == id) {
                    newBoardList[index].timeShown = value

                    this.setState({
                        boardList: newBoardList
                    });
                }
            })
        }
        else {
            this.state.boardList.map((board, index) => {

                if (board.id.toString() == id) {
                    newBoardList[index].refreshRate = value

                    this.setState({
                        boardList: newBoardList
                    });
                }
            })
        }
    }

    public render() {
        if (sessionStorage.getItem('JwtToken') === null) {
            return null;
        }

        let contents = this.state.loading
            ? null
            : EditPresentation.renderBoardList(this.state.boardList, this.handleSubmit, this.handleChangeBoardVisibility, this.handleChangeBoardTimes);

        return <div className="top-padding">

            <h1>Rediģēt prezentāciju</h1>

            <form name="presentation" onSubmit={this.handleForm}>
                <div key="title" className="FormElement">
                    Nosaukums: <input id="title" name="title" type="text" value={this.state.boardPresentation.title} onChange={this.handleChange} />
                </div>
                <div key="owner" className="FormElement">
                    Izveidotājs: <input id="owner" name="owner" type="text" disabled value={this.state.boardPresentation.owner} />
                </div>
                <div key="username" className="FormElement">
                    Lietotājvārds: <input id="username" required name="username" type="text" value={this.state.boardPresentation.credentials.username} onChange={this.handleChange} />
                </div>
                <div key="password" className="FormElement">
                    Parole: <input id="password" required name="password" type="password" value={this.state.boardPresentation.credentials.password} onChange={this.handleChange} />
                </div>
                <div className="FormButton"><button type="submit" className="btn btn-default">Apstiprināt izmaiņas <br /> autentifikācijas datos</button></div>
            </form>

            <h4>Brīdinājums! Lietotājvārds un parole tiks glabāti atklātā tekstā uz servera!</h4>
            <h4 className="Error">{this.state.error}</h4>
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
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Nosaukums</th>
                            <th className="Center">Iekļaut prezentācijā</th>
                            <th className="Center">Attēlošanas laiks(s)</th>
                            <th className="Center">Atjaunošanas laiks(s)</th>
                        </tr>
                    </thead>
                    <tbody>
                        {boardList.map(board =>
                            <tr key={board.id + "row"}>
                                <td key={board.id + ""}>{board.id}</td>
                                <td key={board.id + "name"}>{board.name}</td>
                                <td key={board.id + "visibility"} className="Center"><input name={board.id + "visibility"} type="checkbox" defaultChecked={board.visibility} onClick={() => handleChangeBoardVisibility(board.id)} /></td>
                                <td key={board.id + "timeShown"} className="Center"><input name={board.id + "timeShown"} type="number" min="0" value={board.timeShown.toString()} onChange={(e) => handleChangeBoardTimes(board.id, 'timeShown', e)} /></td>
                                <td key={board.id + "refreshRate"} className="Center"><input name={board.id + "refreshRate"} type="number" min="0" value={board.refreshRate.toString()} onChange={(e) => handleChangeBoardTimes(board.id, 'refreshRate', e)} /></td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <button className="btn btn-default" type="submit">Saglabāt izmaiņas</button>
            </form>
        </div>
    }
}
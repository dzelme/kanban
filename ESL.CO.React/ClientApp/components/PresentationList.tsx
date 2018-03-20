import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { BoardPresentation } from './Interfaces';

interface PresentationState {
    presentationList: BoardPresentation[];
    loading: boolean;
}

export class PresentationList extends React.Component<RouteComponentProps<{}>, PresentationState> {
    constructor() {
        super();

        this.state = { presentationList: [], loading: true };

        this.handleNew = this.handleNew.bind(this);

        function handleErrors(response) {
            if (response.status == 401) {
                open('/login','_self');
            }           
            else if (!response.ok) {
                throw Error(response.statusText);
            }
            return response;
        }

        fetch('api/admin/presentations', {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
            }
        })
            .then(handleErrors)
            .then(response => response.json())
            .then(data => {
                this.setState({ presentationList: data, loading: false });
            });
    }

    handleNew()
    {
        open('/admin/createPresentation','_self');
    }

    public render() {

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : PresentationList.renderPresentationList(this.state.presentationList, this.handleNew);

        return <div>
            {contents}
        </div>;
    }

    private static renderPresentationList(presentationList: BoardPresentation[], handleNew) {
        return <div>
            <h1>Prezentāciju saraksts</h1>
            <button onClick={handleNew} className="btn btn-default">Izveidot jaunu</button>
            <table className='table'>
                <thead style={styleHeader}>
                    <tr>
                        <th>ID</th>
                        <th>Nosaukums</th>
                        <th>Izveidotājs</th>
                        <th>Paneļi</th>
                    </tr>
                </thead>
                <tbody style={styleContent}>
                    {presentationList.map(presentation =>
                        <tr key={presentation.id + "row"}>
                            <td key={presentation.id + ""}>{presentation.id}</td>
                            <td key={presentation.id + "title"}>{presentation.title}</td>
                            <td key={presentation.id + "owner"}>{presentation.owner}</td>
                            <td key={presentation.id + "boards"}>
                                {presentation.boards.values.map(board =>
                                    board.id + "; " 
                                )}
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    }
}

const styleHeader = {
    fontSize: '20px'
}

const styleContent = {
    fontSize: '15px'
}
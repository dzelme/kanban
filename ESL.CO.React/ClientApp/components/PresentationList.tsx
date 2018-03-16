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

        fetch('api/presentations', {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
            }
        })
            .then(response => response.json())
            .then(data => {
                this.setState({ presentationList: data, loading: false });
            });
    }

    public render() {

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : PresentationList.renderPresentationList(this.state.presentationList);

        return <div>

            {contents}
        </div>;
    }

    private static renderPresentationList(presentationList: BoardPresentation[]) {
        return <div>
            <h1>Presentation List</h1>
            <table className='table'>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Title</th>
                        <th>Owner</th>
                        <th>Boards</th>
                    </tr>
                </thead>
                <tbody>
                    {presentationList.map(presentation =>
                        <tr key={presentation.id + "row"}>
                            <td key={presentation.id + ""}>{presentation.id}</td>
                            <td key={presentation.id + "title"}>{presentation.title}</td>
                            <td key={presentation.id + "owner"}>{presentation.owner}</td>
                            <td key={presentation.id + "boards"}>
                                {presentation.boards.values.map(board =>
                                    board.id + " " 
                                )}
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    }


}
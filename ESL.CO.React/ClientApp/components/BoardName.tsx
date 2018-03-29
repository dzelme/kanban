import * as React from 'react';
import { Link } from 'react-router-dom';
import { Value } from './Interfaces';

export default class BoardName extends React.Component<{ presentationId: string, boardId: number, name: string, fromCache: boolean, message: string, boardlist: Value[] }> {

    public render() {

        var nameCount = this.props.boardlist.length;
        var size = 100 / nameCount;
        styleList.width = size + '%';
        styleActive.width = size + '%';

        return <div>
            {
                this.props.boardlist.map((board, index) =>
                    <div style={BoardName.statusColor(this.props.name, board.name)} key={index}><Link to={"/k/" + this.props.presentationId + "/" + board.id}> <h3>{board.name}</h3></Link></div>
                    )
            }
            {this.props.fromCache ? <h4>Dati no keša</h4> : ""}<h4>{this.props.message}</h4>
        </div>
    }

    private static statusColor(currentBoardName: string, boardName: string) {
        let style;

        if (boardName == currentBoardName) {
            style = styleActive;
        }
        else {
            style = styleList;
        }

        return style;
    }

}

const styleList = {
    display: 'inline-block',
    width: '',
    border: 'solid',
    borderColor: 'white',
    textAlign: 'center',
    marginBottom:'30px'
}

const styleActive = {
    display: 'inline-block',
    width: '',
    border: 'solid',
    borderColor: 'white',
    textAlign: 'center',
    background: 'grey',
    color: 'white',
    marginBottom: '30px'
}
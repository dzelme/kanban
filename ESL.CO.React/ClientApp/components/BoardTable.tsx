import * as React from 'react';
import Column from './Column';
import { Board } from './Interfaces';

export default class BoardTable extends React.Component<{ board: Board }> {

    public render() {
        return <tr>
            {this.props.board.columns.map((column, index) =>
                <td key={index} style={styleColumn}>
                    <Column column={column} board={this.props.board} />
                </td>
            )}
        </tr>
    }
}

const styleColumn = {
    border: 'solid',
};